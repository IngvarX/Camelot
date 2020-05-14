using System;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperation : IInternalOperation
    {
        OperationInfo Info { get; }

        event EventHandler<EventArgs> Cancelled;

        Task CancelAsync();
    }
}