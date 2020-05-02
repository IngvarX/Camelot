using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperation
    {
        event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

        event EventHandler<System.EventArgs> OperationFinished;

        Task RunAsync(CancellationToken cancellationToken = default);
    }
}