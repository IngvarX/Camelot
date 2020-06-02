using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.Operations
{
    public class OperationContinuationOptions
    {
        public string FilePath { get; }

        public bool ApplyForAll { get; }

        public OperationContinuationMode Mode { get; }

        public string NewFileName { get; }

        private OperationContinuationOptions(
            string filePath,
            bool applyForAll,
            OperationContinuationMode mode,
            string newFileName = null)
        {
            FilePath = filePath;
            ApplyForAll = applyForAll;
            Mode = mode;
            NewFileName = newFileName;
        }

        public static OperationContinuationOptions CreateRenamingContinuationOptions(
            string filePath, bool applyForAll, string newFileName) =>
            new OperationContinuationOptions(filePath, applyForAll, OperationContinuationMode.Rename, newFileName);

        public static OperationContinuationOptions CreateContinuationOptions(
            string filePath, bool applyForAll, OperationContinuationMode mode) =>
            new OperationContinuationOptions(filePath, applyForAll, mode);
    }
}