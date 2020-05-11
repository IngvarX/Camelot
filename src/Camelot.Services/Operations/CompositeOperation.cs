using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Services.Operations
{
    public class CompositeOperation : OperationBase, IOperation
    {
        private readonly ITaskPool _taskPool;
        private readonly IList<IList<IInternalOperation>> _groupedOperationsToExecute;
        private readonly IList<IList<IInternalOperation>> _groupedOperationsToCancel;

        private IList<IList<IInternalOperation>> _currentOperations;
        private int _finishedOperationsCount;
        private CancellationToken _cancellationToken;

        public OperationInfo OperationInfo { get; }

        public event EventHandler<EventArgs> OperationCancelled;

        public CompositeOperation(ITaskPool taskPool,
            IList<IList<IInternalOperation>> groupedOperationsToExecute,
            IList<IList<IInternalOperation>> groupedOperationsToCancel,
            OperationInfo operationInfo)
        {
            _taskPool = taskPool;
            _groupedOperationsToExecute = groupedOperationsToExecute;
            _groupedOperationsToCancel = groupedOperationsToCancel;
            OperationInfo = operationInfo;
        }

        public override Task RunAsync(CancellationToken cancellationToken) =>
            ExecuteOperationsAsync(_groupedOperationsToExecute, cancellationToken);

        public async Task CancelAsync()
        {
            // TODO: stop operation
            await ExecuteOperationsAsync(_groupedOperationsToCancel, cancellationToken);

            OperationCancelled.Raise(this, EventArgs.Empty);
        }

        private async Task ExecuteOperationsAsync(
            IList<IList<IInternalOperation>> groupedOperationsToExecute,
            CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _finishedOperationsCount = 0;
            _currentOperations = groupedOperationsToExecute;

            foreach (var operationsGroup in _currentOperations)
            {
                var operationsCount = operationsGroup.Count;
                for (var i = 0; i < operationsCount; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentOperation = operationsGroup[i];
                    SubscribeToEvents(currentOperation);

                    await _taskPool.ExecuteAsync(() => currentOperation.RunAsync(cancellationToken));
                }
            }
        }

        private void CurrentOperationOnOperationFinished(object sender, System.EventArgs e)
        {
            var operation = (IInternalOperation) sender;
            UnsubscribeFromEvents(operation);

            var finishedOperationsCount = Interlocked.Increment(ref _finishedOperationsCount);
            var totalOperationsCount = _currentOperations.Sum(g => g.Count);

            if (finishedOperationsCount == totalOperationsCount)
            {
                FireOperationFinishedEvent();
            }
            else
            {
                FireProgressChangedEvent((double) finishedOperationsCount / totalOperationsCount);
            }
        }

        private void SubscribeToEvents(IInternalOperation currentOperation)
        {
            currentOperation.OperationFinished += CurrentOperationOnOperationFinished;
        }

        private void UnsubscribeFromEvents(IInternalOperation currentOperation)
        {
            currentOperation.OperationFinished -= CurrentOperationOnOperationFinished;
        }
    }
}