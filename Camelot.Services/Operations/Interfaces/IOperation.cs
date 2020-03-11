using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.EventArgs;

namespace Camelot.Services.Operations.Interfaces
{
    public interface IOperation
    {
        public event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

        public event EventHandler<System.EventArgs> OperationFinished;

        public Task RunAsync(CancellationToken cancellationToken = default);
    }
}