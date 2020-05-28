using System;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperationBase
    {
        OperationState State { get; }

        double CurrentProgress { get; }

        event EventHandler<OperationStateChangedEventArgs> StateChanged;

        event EventHandler<OperationProgressChangedEventArgs> ProgressChanged;
    }
}