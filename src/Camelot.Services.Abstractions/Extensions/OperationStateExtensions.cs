using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Extensions
{
    public static class OperationStateExtensions
    {
        public static bool IsCompleted(this OperationState operationState)
        {
            var completedOperationStates = new[]
            {
                OperationState.Failed,
                OperationState.Cancelled,
                OperationState.Finished,
                OperationState.Skipped
            };

            return completedOperationStates.Contains(operationState);
        }

        public static bool IsFailedOrCancelled(this OperationState operationState) =>
            operationState is OperationState.Failed || operationState is OperationState.Cancelled;

        public static bool IsCancellationAvailable(this OperationState operationState)
        {
            var cancellableOperationStates = new[]
            {
                OperationState.InProgress,
                OperationState.Paused,
                OperationState.Pausing,
                OperationState.Unpausing,
                OperationState.Finished
            };

            return cancellableOperationStates.Contains(operationState);
        }
    }
}