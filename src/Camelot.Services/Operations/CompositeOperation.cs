using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Services.Operations
{
    public class CompositeOperation : OperationBase, IOperation
    {
        private readonly ITaskPool _taskPool;
        private readonly IReadOnlyList<OperationGroup> _groupedOperationsToExecute;

        private int _finishedOperationsCount;
        private int _groupOperationsCount;
        private int _totalOperationsCount;
        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _taskCompletionSource;

        public OperationInfo Info { get; }

        public CompositeOperation(
            ITaskPool taskPool,
            IReadOnlyList<OperationGroup> groupedOperationsToExecute,
            OperationInfo operationInfo)
        {
            _taskPool = taskPool;
            _groupedOperationsToExecute = groupedOperationsToExecute;
            Info = operationInfo;
        }

        public async Task RunAsync()
        {
            if (OperationState != OperationState.NotStarted)
            {
                throw new InvalidOperationException();
            }

            OperationState = OperationState.InProgress;
            var operations = _groupedOperationsToExecute.Select(g => g.Operations).ToArray();

            await ExecuteOperationsAsync(operations);
        }

        public Task ContinueAsync(OperationContinuationOptions options)
        {
            if (OperationState != OperationState.Blocked)
            {
                throw new InvalidOperationException();
            }

            // TODO: add continue
            return Task.CompletedTask;
        }

        public async Task CancelAsync()
        {
            _cancellationTokenSource.Cancel();

            var cancelOperations = _groupedOperationsToExecute
                .Reverse() // TODO: skip blocked operations
                .Where((o, i) => o.Operations[i].OperationState != OperationState.NotStarted)
                .Select(g => g.CancelOperations)
                .ToArray();

            await ExecuteOperationsAsync(cancelOperations);

            OperationState = OperationState.Cancelled;
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
                _groupOperationsCount = operationsGroup.Count;

                for (var i = 0; i < _groupOperationsCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentOperation = operationsGroup[i];
                    SubscribeToEvents(currentOperation);

                    await _taskPool.ExecuteAsync(() => currentOperation.RunAsync(cancellationToken));
                }

                await _taskCompletionSource.Task;
            }

            OperationState = OperationState.Finished;
        }

        private void CurrentOperationOnStateChanged(object sender, OperationStateChangedEventArgs e)
        {
            if (e.OperationState != OperationState.Finished)
            {
                return;
            }

            var operation = (IInternalOperation) sender;
            UnsubscribeFromEvents(operation);

            var finishedOperationsCount = Interlocked.Increment(ref _finishedOperationsCount);
            if (finishedOperationsCount == _groupOperationsCount)
            {
                _taskCompletionSource.SetResult(true);
            }

            // TODO: fix
            CurrentProgress = (double) finishedOperationsCount / _totalOperationsCount;
        }

        private void SubscribeToEvents(IInternalOperation currentOperation)
        {
            currentOperation.StateChanged += CurrentOperationOnStateChanged;
        }

        private void UnsubscribeFromEvents(IInternalOperation currentOperation)
        {
            currentOperation.StateChanged -= CurrentOperationOnStateChanged;
        }
    }
}