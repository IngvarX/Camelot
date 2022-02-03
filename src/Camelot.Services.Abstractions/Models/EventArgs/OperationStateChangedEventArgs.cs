using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.EventArgs;

public class OperationStateChangedEventArgs : System.EventArgs
{
    public OperationState OperationState { get; }

    public OperationStateChangedEventArgs(OperationState operationState)
    {
        OperationState = operationState;
    }
}