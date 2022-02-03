using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Operations;

public interface ISuspendableOperation
{
    Task PauseAsync();

    Task UnpauseAsync();
}