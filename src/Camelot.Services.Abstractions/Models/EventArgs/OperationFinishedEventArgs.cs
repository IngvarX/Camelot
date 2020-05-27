using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.EventArgs
{
    public class OperationFinishedEventArgs : System.EventArgs
    {
        public OperationResult OperationResult { get; }

        public OperationFinishedEventArgs(OperationResult operationResult)
        {
            OperationResult = operationResult;
        }
    }
}