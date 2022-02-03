using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations;

public interface ISelfBlockingOperation
{
    (string SourceFilePath, string DestinationFilePath) CurrentBlockedFile { get; }

    Task ContinueAsync(OperationContinuationOptions options);
}