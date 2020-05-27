using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Extensions
{
    public static class OperationStateExtensions
    {
        public static bool IsCompleted(this OperationState operationState) =>
            operationState != OperationState.NotStarted && operationState != OperationState.InProgress;

        public static bool IsCancellationAvailable(this OperationState operationState)
        {
            var operationWithoutCancellation = new[]
                {OperationState.NotStarted, OperationState.Blocked, OperationState.Failed};

            return !operationWithoutCancellation.Contains(operationState);
        }
    }
}