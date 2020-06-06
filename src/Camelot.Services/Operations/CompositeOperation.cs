using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
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
        private readonly IReadOnlyList<OperationGroup> _groupedOperationsToExecute;

        private readonly IDictionary<string, ISelfBlockingOperation> _blockedOperationsDictionary;
        private readonly IList<(string SourceFilePath, string DestinationFilePath)> _blockedFiles;

        private int _finishedOperationsCount;
        private IReadOnlyList<IInternalOperation> _currentOperationsGroup;
        private int _totalOperationsCount;
        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _taskCompletionSource;
        private OperationContinuationOptions _options;

        public OperationInfo Info { get; }

        public IReadOnlyList<(string SourceFilePath, string DestinationFilePath)> BlockedFiles => _blockedFiles.ToArray();

        public event EventHandler<EventArgs> Blocked;

        public CompositeOperation(
            ITaskPool taskPool,
            IReadOnlyList<OperationGroup> groupedOperationsToExecute,
            OperationInfo operationInfo)
        {
            _taskPool = taskPool;
            _groupedOperationsToExecute = groupedOperationsToExecute;

            _blockedOperationsDictionary = new ConcurrentDictionary<string, ISelfBlockingOperation>();
            _blockedFiles = new List<(string SourceFilePath, string DestinationFilePath)>();
            Info = operationInfo;
        }

        public async Task RunAsync()
        {
            var operations = _groupedOperationsToExecute.Select(g => g.Operations).ToArray();

            await ExecuteOperationsAsync(operations);
        }

        public async Task ContinueAsync(OperationContinuationOptions options)
        {
            _options = options;
            var operation = _blockedOperationsDictionary[options.FilePath];

            await operation.ContinueAsync(options);
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

        private async void CurrentOperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {
            var state = e.OperationState;

            if (state is OperationState.Blocked)
            {
                var operation = (ISelfBlockingOperation) sender;

                if (_options?.ApplyForAll == true)
                {
                    // TODO: process renaming
                    await operation.ContinueAsync(_options);
                }
                else
                {
                    operation
                        .BlockedFiles
                        .ForEach(f =>
                        {
                            _blockedOperationsDictionary.Add(f.SourceFilePath, operation);
                            _blockedFiles.Add(f);
                        });
                    Blocked.Raise(this, EventArgs.Empty);
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