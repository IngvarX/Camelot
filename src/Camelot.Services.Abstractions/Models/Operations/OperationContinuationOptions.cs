using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.Operations;

public class OperationContinuationOptions
{
    public string FilePath { get; }

    public bool ApplyToAll { get; }

    public OperationContinuationMode Mode { get; }

    public string NewFilePath { get; }

    private OperationContinuationOptions(
        string filePath,
        bool applyToAll,
        OperationContinuationMode mode,
        string newFilePath = null)
    {
        FilePath = filePath;
        ApplyToAll = applyToAll;
        Mode = mode;
        NewFilePath = newFilePath;
    }

    public static OperationContinuationOptions CreateRenamingContinuationOptions(
        string filePath, bool applyToAll, string newFilePath) =>
        new OperationContinuationOptions(filePath, applyToAll, OperationContinuationMode.Rename, newFilePath);

    public static OperationContinuationOptions CreateContinuationOptions(
        string filePath, bool applyToAll, OperationContinuationMode mode) =>
        new OperationContinuationOptions(filePath, applyToAll, mode);
}