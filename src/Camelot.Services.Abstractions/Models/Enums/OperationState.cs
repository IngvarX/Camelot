using System.Net.NetworkInformation;

namespace Camelot.Services.Abstractions.Models.Enums
{
    public enum OperationState : byte
    {
        NotStarted,
        InProgress,
        Finished,
        Cancelled,
        Failed
    }
}