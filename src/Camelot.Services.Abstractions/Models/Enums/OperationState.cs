namespace Camelot.Services.Abstractions.Models.Enums
{
    public enum OperationState : byte
    {
        NotStarted,
        InProgress,
        Paused,
        Blocked,
        Finished,
        Cancelled,
        Failed
    }
}