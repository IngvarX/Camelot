using System;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperation : IOperationBase
    {
        OperationInfo Info { get; }

        Task RunAsync();

        Task ContinueAsync(OperationContinuationOptions options);

        Task CancelAsync();
    }
}