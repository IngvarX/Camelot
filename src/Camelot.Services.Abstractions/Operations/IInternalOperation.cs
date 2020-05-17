using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IInternalOperation
    {
        OperationState OperationState { get; }

        double CurrentProgress { get; }

        event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;

        event EventHandler<OperationStateChangedEventArgs> StateChanged;

        Task RunAsync(CancellationToken cancellationToken = default);
    }
}