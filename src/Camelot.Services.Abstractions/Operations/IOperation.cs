using System;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperation : IInternalOperation
    {
        OperationInfo OperationInfo { get; }

        event EventHandler<EventArgs> OperationCancelled;

        Task CancelAsync();
    }
}