using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Extensions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Services.Operations
{
    public class CompositeOperation : OperationBase, ICompositeOperation
    {
        private readonly ITaskPool _taskPool;
        private readonly IFileNameGenerationService _fileNameGenerationService;
        private readonly IReadOnlyList<OperationGroup> _groupedOperationsToExecute;

        private readonly IDictionary<string, ISelfBlockingOperation> _blockedOperationsDictionary;
        private readonly Queue<(string SourceFilePath, string DestinationFilePath)> _blockedFilesQueue;

        private int _finishedOperationsCount;
        private IReadOnlyList<IInternalOperation> _currentOperationsGroup;
        private int _totalOperationsCount;
        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _taskCompletionSource;
        private OperationContinuationMode? _continuationMode;

        public OperationInfo Info { get; }

        public (string SourceFilePath, string DestinationFilePath) BlockedFile { get; private set; }

        public event EventHandler<EventArgs> Blocked;

        public CompositeOperation(
            ITaskPool taskPool,
            IFileNameGenerationService fileNameGenerationService,
            IReadOnlyList<OperationGroup> groupedOperationsToExecute,
            OperationInfo operationInfo)
        {
            _taskPool = taskPool;
            _fileNameGenerationService = fileNameGenerationService;
            _groupedOperationsToExecute = groupedOperationsToExecute;

            _blockedOperationsDictionary = new ConcurrentDictionary<string, ISelfBlockingOperation>();
            _blockedFilesQueue = new Queue<(string SourceFilePath, string DestinationFilePath)>();
            Info = operationInfo;
        }

        public async Task RunAsync()
        {
            var operations = _groupedOperationsToExecute.Select(g => g.Operations).ToArray();

            await ExecuteOperationsAsync(operations);
        }

        public async Task ContinueAsync(OperationContinuationOptions options)
        {
            if (options.ApplyForAll)
            {
                _continuationMode = options.Mode;
            }

            BlockedFile = default;

            var operation = _blockedOperationsDictionary[options.FilePath];
            _blockedOperationsDictionary.Remove(options.FilePath);

            await operation.ContinueAsync(options);

            // TODO: process with default mode?
            if (_blockedFilesQueue.Any())
            {
                BlockedFile = _blockedFilesQueue.Dequeue();

                Blocked.Raise(this, EventArgs.Empty);
            }
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
                .Where((o, i) => o.Operations[i].State.IsCancellationAvailable())
                .Select(g => g.CancelOperations)
                .ToArray();

            await ExecuteOperationsAsync(cancelOperations);
        }

        private async Task ExecuteOperationsAsync(
            IReadOnlyList<IReadOnlyList<IInternalOperation>> groupedOperationsToExecute)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            _totalOperationsCount = groupedOperationsToExecute.Sum(g => g.Count);

            foreach (var operationsGroup in groupedOperationsToExecute)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _taskCompletionSource = new TaskCompletionSource<bool>();

                _finishedOperationsCount = 0;
                _currentOperationsGroup = operationsGroup;

                for (var i = 0; i < _currentOperationsGroup.Count; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentOperation = operationsGroup[i];
                    SubscribeToEvents(currentOperation);

                    await _taskPool.ExecuteAsync(() => currentOperation.RunAsync(cancellationToken));
                }

                await _taskCompletionSource.Task;
            }
        }

        // TODO: refactor
        private async void CurrentOperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {
            var state = e.OperationState;

            if (state is OperationState.Blocked)
            {
                var operation = (ISelfBlockingOperation) sender;

                if (_continuationMode is null)
                {
                    _blockedOperationsDictionary.Add(operation.BlockedFile.SourceFilePath, operation);

                    if (BlockedFile != default)
                    {
                        _blockedFilesQueue.Enqueue(operation.BlockedFile);

                        return;
                    }

                    BlockedFile = operation.BlockedFile;

                    Blocked.Raise(this, EventArgs.Empty);
                }
                else
                {
                    var (sourceFilePath, destinationFilePath) = operation.BlockedFile;
                    OperationContinuationOptions options;
                    if (_continuationMode is OperationContinuationMode.Rename)
                    {
                        var newFilePath = _fileNameGenerationService.GenerateName(destinationFilePath);
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

                return;
            }

            if (state.IsCompleted())
            {
                var operation = (IInternalOperation) sender;
                UnsubscribeFromEvents(operation);

                var finishedOperationsCount = Interlocked.Increment(ref _finishedOperationsCount);
                if (finishedOperationsCount == _currentOperationsGroup.Count)
                {
                    _taskCompletionSource.SetResult(true);
                }
            }

            UpdateProgress();
        }

        private void SubscribeToEvents(IInternalOperation currentOperation)
        {
            currentOperation.StateChanged += CurrentOperationOnStateChanged;
            currentOperation.ProgressChanged += CurrentOperationOnProgressChanged;
        }


        private void UnsubscribeFromEvents(IInternalOperation currentOperation)
        {
            currentOperation.StateChanged -= CurrentOperationOnStateChanged;
            currentOperation.ProgressChanged += CurrentOperationOnProgressChanged;
        }

        private void CurrentOperationOnProgressChanged(object sender, OperationProgressChangedEventArgs e) =>
            UpdateProgress();

        private void UpdateProgress()
        {
            // TODO: prev group?
            CurrentProgress = _currentOperationsGroup.Sum(o => o.CurrentProgress) / _totalOperationsCount;
        }
    }
}