using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Operations
{
    public interface ICompositeOperation : ISuspendableOperation, IOperationWithProgress, IOperationWithInfo,
        ISelfBlockingOperation
    {
        Task RunAsync();

        Task CancelAsync();
    }
}