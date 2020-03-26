using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.EventArgs;
using Camelot.Services.Operations.Interfaces;

namespace Camelot.Services.Operations.Implementations
{
    public abstract class OperationBase : IOperation
    {
        public event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

        public event EventHandler<System.EventArgs> OperationFinished;

        public abstract Task RunAsync(CancellationToken cancellationToken);

        protected void FireProgressChangedEvent(double currentProgress)
        {
            var args = new OperationProgressChangedEventArgs(currentProgress);

            ProgressChanged.Raise(this, args);
        }

        protected void FireOperationFinishedEvent()
        {
            OperationFinished.Raise(this, System.EventArgs.Empty);
        }
    }
}