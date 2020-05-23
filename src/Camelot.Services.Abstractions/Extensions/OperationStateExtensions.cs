using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Extensions
{
    public static class OperationStateExtensions
    {
        public static bool IsCompleted(this OperationState operationState) =>
            operationState != OperationState.NotStarted && operationState != OperationState.InProgress;
    }
}