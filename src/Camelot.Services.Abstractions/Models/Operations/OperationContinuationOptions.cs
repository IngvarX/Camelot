using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.Operations
{
    public class OperationContinuationOptions
    {
        public bool ApplyForAll { get; }

        public OperationContinuationMode Mode { get; }

        public string NewFileName { get; }

        private OperationContinuationOptions(
            bool applyForAll,
            OperationContinuationMode mode,
            string newFileName = null)
        {
            ApplyForAll = applyForAll;
            Mode = mode;
            NewFileName = newFileName;
        }

        public static OperationContinuationOptions CreateRenamingContinuationOptions(bool applyForAll, string newFileName) =>
            new OperationContinuationOptions(applyForAll, OperationContinuationMode.Rename, newFileName);

        public static OperationContinuationOptions CreateContinuationOptions(bool applyForAll, OperationContinuationMode mode) =>
            new OperationContinuationOptions(applyForAll, mode);
    }
}