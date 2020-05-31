using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface ISuspendableOperation
    {
        OperationInfo Info { get; }

        Task RunAsync();

        Task ContinueAsync(OperationContinuationOptions options);

        Task CancelAsync();
    }
}