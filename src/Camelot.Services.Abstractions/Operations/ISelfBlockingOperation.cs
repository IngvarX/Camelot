using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface ISelfBlockingOperation
    {
        IReadOnlyCollection<string> BlockedFiles { get; }

        Task ContinueAsync(OperationContinuationOptions options);
    }
}