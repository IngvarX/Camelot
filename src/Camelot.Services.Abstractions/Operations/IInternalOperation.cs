using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Operations;

public interface IInternalOperation : IOperationWithProgress, IStatefulOperation
{
    Task RunAsync(CancellationToken cancellationToken);
}