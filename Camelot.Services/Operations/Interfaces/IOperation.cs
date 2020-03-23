using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.EventArgs;

namespace Camelot.Services.Operations.Interfaces
{
    public interface IOperation
    {
        event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

        event EventHandler<System.EventArgs> OperationFinished;

        Task RunAsync(CancellationToken cancellationToken = default);
    }
}