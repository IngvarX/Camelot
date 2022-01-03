namespace Camelot.Services.Abstractions.Models.Enums;

public enum OperationContinuationMode : byte
{
    Skip,
    Overwrite,
    OverwriteIfOlder,
    Rename
}