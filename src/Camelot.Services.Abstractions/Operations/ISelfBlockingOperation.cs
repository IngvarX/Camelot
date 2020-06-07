using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface ISelfBlockingOperation
    {
        (string SourceFilePath, string DestinationFilePath) BlockedFile { get; }

        Task ContinueAsync(OperationContinuationOptions options);
    }
}