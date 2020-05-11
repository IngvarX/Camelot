using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IInternalOperation
    {
        event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

        event EventHandler<EventArgs> OperationFinished;

        Task RunAsync(CancellationToken cancellationToken = default);
    }
}