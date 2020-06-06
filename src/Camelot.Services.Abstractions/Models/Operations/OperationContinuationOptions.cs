using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.Operations
{
    public class OperationContinuationOptions
    {
        public string FilePath { get; }

        public bool ApplyForAll { get; }

        public OperationContinuationMode Mode { get; }

        public string NewFilePath { get; }

        private OperationContinuationOptions(
            string filePath,
            bool applyForAll,
            OperationContinuationMode mode,
            string newFilePath = null)
        {
            FilePath = filePath;
            ApplyForAll = applyForAll;
            Mode = mode;
            NewFilePath = newFilePath;
        }

        public static OperationContinuationOptions CreateRenamingContinuationOptions(
            string filePath, bool applyForAll, string newFilePath) =>
            new OperationContinuationOptions(filePath, applyForAll, OperationContinuationMode.Rename, newFilePath);

        public static OperationContinuationOptions CreateContinuationOptions(
            string filePath, bool applyForAll, OperationContinuationMode mode) =>
            new OperationContinuationOptions(filePath, applyForAll, mode);
    }
}