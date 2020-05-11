using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public abstract class OperationBase : IInternalOperation
    {
        public event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

        public event EventHandler<EventArgs> OperationFinished;

        public abstract Task RunAsync(CancellationToken cancellationToken);

        protected void FireProgressChangedEvent(double currentProgress)
        {
            var args = new OperationProgressChangedEventArgs(currentProgress);

            ProgressChanged.Raise(this, args);
        }

        protected void FireOperationFinishedEvent() => OperationFinished.Raise(this, EventArgs.Empty);
    }
}