using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Abstractions.Models.EventArgs
{
    public class OperationFinishedEventArgs : System.EventArgs
    {
        public IOperation Operation { get; }

        public OperationResult OperationResult { get; }

        public OperationFinishedEventArgs(IOperation operation, OperationResult operationResult)
        {
            Operation = operation;
            OperationResult = operationResult;
        }
    }
}