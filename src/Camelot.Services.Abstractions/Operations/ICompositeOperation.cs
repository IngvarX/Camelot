using System;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions.Operations;

public interface ICompositeOperation : ISuspendableOperation, IOperationWithProgress, IOperationWithInfo,
    ISelfBlockingOperation
{
    event EventHandler<EventArgs> Blocked;

    Task RunAsync();

    Task CancelAsync();
}