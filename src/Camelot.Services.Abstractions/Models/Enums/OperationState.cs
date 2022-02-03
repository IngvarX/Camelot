namespace Camelot.Services.Abstractions.Models.Enums;

public enum OperationState : byte
{
    NotStarted,
    InProgress,
    Paused,
    Pausing,
    Unpausing,
    Blocked,
    Finished,
    Cancelling,
    Cancelled,
    Failed,
    Skipped
}