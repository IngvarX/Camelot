using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Operations;

public interface IOperation : ISuspendableOperation, IOperationWithProgress, IOperationWithInfo,
    ISelfBlockingOperation, IStatefulOperation
{
    Task RunAsync();

    Task CancelAsync();
}