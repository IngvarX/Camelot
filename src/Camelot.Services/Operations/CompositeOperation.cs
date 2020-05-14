using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
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
        private CancellationToken _cancellationToken;
        private TaskCompletionSource<bool> _taskCompletionSource;

        public OperationInfo Info { get; }

        public event EventHandler<EventArgs> Cancelled;

        public CompositeOperation(ITaskPool taskPool,
            IReadOnlyList<OperationGroup> groupedOperationsToExecute,
            OperationInfo operationInfo)
        {
            _taskPool = taskPool;
            _groupedOperationsToExecute = groupedOperationsToExecute;
            Info = operationInfo;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken) =>
            ExecuteOperationsAsync(_groupedOperationsToExecute.Select(g => g.Operations).ToArray(),
                cancellationToken);

        public async Task CancelAsync()
        {
            var cancelOperations = _groupedOperationsToExecute
                .Reverse()
                .Where((o, i) => o.Operations[i].OperationState != OperationState.NotStarted)
                .Select(g => g.CancelOperations)
                .ToArray();

            await ExecuteOperationsAsync(cancelOperations, _cancellationToken);

            Cancelled.Raise(this, EventArgs.Empty);
        }

        private async Task ExecuteOperationsAsync(
            IReadOnlyList<IReadOnlyList<IInternalOperation>> groupedOperationsToExecute,
            CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
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

            FireProgressChangedEvent((double) finishedOperationsCount / _totalOperationsCount);
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