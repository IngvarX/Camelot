using System.Linq;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Extensions;

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
    
    public static bool IsSuccessful(this OperationState operationState) => 
        operationState is OperationState.Finished or OperationState.Skipped;

    public static bool IsFailedOrCancelled(this OperationState operationState) =>
        operationState is OperationState.Failed or OperationState.Cancelled;
}