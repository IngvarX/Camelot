using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IInternalOperation : IOperationBase
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}