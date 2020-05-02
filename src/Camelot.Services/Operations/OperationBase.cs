using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Models.EventArgs;

namespace Camelot.Services.Operations
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