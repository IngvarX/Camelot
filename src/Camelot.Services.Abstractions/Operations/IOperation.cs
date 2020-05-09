using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperation
    {
        event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

        event EventHandler<EventArgs> OperationFinished;

        event EventHandler<EventArgs> OperationCancelled;

        Task RunAsync(CancellationToken cancellationToken = default);
    }
}