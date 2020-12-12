using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Operations.Models;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Exceptions;
using Camelot.Services.Abstractions.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Operations
{
    public class CompositeOperation : OperationBase, ICompositeOperation
    {
        private readonly IFileNameGenerationService _fileNameGenerationService;
        private readonly IReadOnlyList<OperationGroup> _groupedOperationsToExecute;

        private readonly IDictionary<string, ISelfBlockingOperation> _blockedOperationsDictionary;
        private readonly ConcurrentQueue<(string SourceFilePath, string DestinationFilePath)> _blockedFilesQueue;
        private readonly object _blockedFileLocker;

        private int _finishedOperationsCount;
        private int _currentOperationsGroupIndex;
        private int _operationsGroupsCount;
        private IReadOnlyList<IInternalOperation> _currentOperationsGroup;
        private int _totalOperationsCount;
        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _taskCompletionSource;
        private OperationContinuationMode? _continuationMode;

        public OperationInfo Info { get; }

        public (string SourceFilePath, string DestinationFilePath) CurrentBlockedFile { get; private set; }

        public event EventHandler<EventArgs> Blocked;

        public CompositeOperation(
            IFileNameGenerationService fileNameGenerationService,
            IReadOnlyList<OperationGroup> groupedOperationsToExecute,
            OperationInfo operationInfo)
        {
            _fileNameGenerationService = fileNameGenerationService;
            _groupedOperationsToExecute = groupedOperationsToExecute;
            Info = operationInfo;

            _blockedOperationsDictionary = new ConcurrentDictionary<string, ISelfBlockingOperation>();
            _blockedFilesQueue = new ConcurrentQueue<(string SourceFilePath, string DestinationFilePath)>();
            _blockedFileLocker = new object();
        }

        public async Task RunAsync()
        {
            var operations = _groupedOperationsToExecute.Select(g => g.Operations).ToArray();

            await ExecuteOperationsAsync(operations);
        }

        public async Task ContinueAsync(OperationContinuationOptions options)
        {
            if (options.ApplyToAll)
            {
                _continuationMode = options.Mode;
            }

            lock (_blockedFileLocker)
            {
                CurrentBlockedFile = default;
            }

            var operation = _blockedOperationsDictionary[options.FilePath];
            _blockedOperationsDictionary.Remove(options.FilePath);

            await operation.ContinueAsync(options);
            await ProcessNextBlockedTaskAsync();
        }

        public Task PauseAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task UnpauseAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task CancelAsync()
        {
            _cancellationTokenSource.Cancel();
            // TODO: wait?

            var cancelOperations = _groupedOperationsToExecute
                .Reverse()
                .Where(g => g.IsCancellationAvailable)
                .Select(g =>
                    g.CancelOperations
                        .Where((o, i) => g.Operations[i].State.IsCancellationAvailable()).ToArray())
                        .ToArray();

            await ExecuteOperationsAsync(cancelOperations);
        }

        private async Task ExecuteOperationsAsync(
            IReadOnlyList<IReadOnlyList<IInternalOperation>> groupedOperationsToExecute)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            _totalOperationsCount = groupedOperationsToExecute.Sum(g => g.Count);
            _operationsGroupsCount = groupedOperationsToExecute.Count;

            foreach (var operationsGroup in groupedOperationsToExecute)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!operationsGroup.Any())
                {
                    continue;
                }

                _taskCompletionSource = new TaskCompletionSource<bool>();

                _finishedOperationsCount = 0;
                _currentOperationsGroup = operationsGroup;

                for (var i = 0; i < _currentOperationsGroup.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentOperation = operationsGroup[i];
                    SubscribeToEvents(currentOperation);

                    RunOperation(currentOperation, cancellationToken);
                }

                var groupExecutionResult = await _taskCompletionSource.Task;
                if (!groupExecutionResult)
                {
                    throw new OperationFailedException();
                }

                _currentOperationsGroupIndex++;
            }

            _currentOperationsGroup = null;
            SetFinalProgress();
        }

        private static void RunOperation(IInternalOperation operation, CancellationToken cancellationToken) =>
            Task.Run(() => operation.RunAsync(cancellationToken), cancellationToken).Forget();

        private async void OperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {
            var state = e.OperationState;

            if (state is OperationState.Blocked)
            {
                var operation = (ISelfBlockingOperation) sender;
                await ProcessBlockedTaskAsync(operation);

                return;
            }

            if (state.IsCompleted())
            {
                var operation = (IInternalOperation) sender;
                UnsubscribeFromEvents(operation);

                var finishedOperationsCount = Interlocked.Increment(ref _finishedOperationsCount);
                if (finishedOperationsCount == _currentOperationsGroup.Count)
                {
                    var isSuccessful = !state.IsFailedOrCancelled();

                    _taskCompletionSource.SetResult(isSuccessful);
                }
            }

            UpdateProgress();
        }

        private async Task ProcessNextBlockedTaskAsync()
        {
            var isBlockedFileAvailable = _blockedFilesQueue.TryDequeue(out var currentBlockedFile);
            if (!isBlockedFileAvailable)
            {
                return;
            }

            var operation = _blockedOperationsDictionary[currentBlockedFile.SourceFilePath];

            await ProcessBlockedTaskAsync(operation);
        }

        private async Task ProcessBlockedTaskAsync(ISelfBlockingOperation operation)
        {
            if (_continuationMode is null)
            {
                _blockedOperationsDictionary[operation.CurrentBlockedFile.SourceFilePath] = operation;

                lock (_blockedFileLocker)
                {
                    if (CurrentBlockedFile != default)
                    {
                        _blockedFilesQueue.Enqueue(operation.CurrentBlockedFile);

                        return;
                    }

                    CurrentBlockedFile = operation.CurrentBlockedFile;
                }

                Blocked.Raise(this, EventArgs.Empty);
            }
            else
            {
                var (sourceFilePath, destinationFilePath) = operation.CurrentBlockedFile;
                OperationContinuationOptions options;
                if (_continuationMode is OperationContinuationMode.Rename)
                {
                    var newFilePath = _fileNameGenerationService.GenerateFullName(destinationFilePath);
                    options = OperationContinuationOptions.CreateRenamingContinuationOptions(
                        sourceFilePath,
                        true,
                        newFilePath
                    );
                }
                else
                {
                    options = OperationContinuationOptions.CreateContinuationOptions(
                        sourceFilePath,
                        true,
                        _continuationMode.Value
                    );
                }

                await operation.ContinueAsync(options);
            }
        }

        private void SubscribeToEvents(IInternalOperation currentOperation)
        {
            currentOperation.StateChanged += OperationOnStateChanged;
            currentOperation.ProgressChanged += OperationOnProgressChanged;
        }

        private void UnsubscribeFromEvents(IInternalOperation currentOperation)
        {
            currentOperation.StateChanged -= OperationOnStateChanged;
            currentOperation.ProgressChanged += OperationOnProgressChanged;
        }

        private void OperationOnProgressChanged(object sender, OperationProgressChangedEventArgs e) =>
            UpdateProgress();

        private void UpdateProgress()
        {
            if (_currentOperationsGroup is null)
            {
                return;
            }

            var finishedOperationGroupsProgress = (double) _currentOperationsGroupIndex;
            var currentOperationGroupProgress = _currentOperationsGroup.Sum(o => o.CurrentProgress) / _totalOperationsCount;

            CurrentProgress = (finishedOperationGroupsProgress + currentOperationGroupProgress) / _operationsGroupsCount;
        }
    }
}