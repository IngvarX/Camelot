using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Operations.Interfaces;

namespace Camelot.Services.Operations.Implementations
{
    public class CompositeOperation : OperationBase
    {
        private readonly IList<IOperation> _operations;

        public CompositeOperation(IList<IOperation> operations)
        {
            _operations = operations;
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: use task pool
            var operationsCount = _operations.Count;
            for (var i = 0; i < operationsCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _operations[i].RunAsync(cancellationToken);

                FireProgressChangedEvent((double) i / operationsCount);
            }

            FireOperationFinishedEvent();
        }
    }
}