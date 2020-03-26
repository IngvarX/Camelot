using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Operations.Interfaces;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Services.Operations.Implementations
{
    public class CompositeOperation : OperationBase
    {
        private readonly ITaskPool _taskPool;
        private readonly IList<IOperation> _operations;

        private int _finishedOperationsCount;

        public CompositeOperation(ITaskPool taskPool, IList<IOperation> operations)
        {
            _taskPool = taskPool;
            _operations = operations;
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            _finishedOperationsCount = 0;

            var operationsCount = _operations.Count;
            for (var i = 0; i < operationsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentOperation = _operations[i];
                currentOperation.OperationFinished += CurrentOperationOnOperationFinished;

                await _taskPool.ExecuteAsync(() => currentOperation.RunAsync(cancellationToken));
            }
        }

        private void CurrentOperationOnOperationFinished(object sender, System.EventArgs e)
        {
            var finishedOperationsCount = Interlocked.Increment(ref _finishedOperationsCount);
            var totalOperationsCount = _operations.Count;

            if (finishedOperationsCount == totalOperationsCount)
            {
                FireOperationFinishedEvent();
            }
            else
            {
                FireProgressChangedEvent((double)finishedOperationsCount / totalOperationsCount);
            }
        }
    }
}