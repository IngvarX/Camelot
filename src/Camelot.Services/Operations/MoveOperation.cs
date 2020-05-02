using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public class MoveOperation : OperationBase
    {
        private readonly IOperation _copyOperation;
        private readonly IOperation _deleteOperation;

        public MoveOperation(IOperation copyOperation, IOperation deleteOperation)
        {
            _copyOperation = copyOperation;
            _deleteOperation = deleteOperation;
        }

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            await _copyOperation.RunAsync(cancellationToken);
            FireProgressChangedEvent(0.5);
            await _deleteOperation.RunAsync(cancellationToken);
            FireOperationFinishedEvent();
        }
    }
}