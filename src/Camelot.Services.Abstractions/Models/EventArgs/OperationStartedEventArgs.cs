using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Abstractions.Models.EventArgs;

public class OperationStartedEventArgs : System.EventArgs
{
    public IOperation Operation { get; }

    public OperationStartedEventArgs(IOperation operation)
    {
        Operation = operation;
    }
}