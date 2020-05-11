using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.Operations
{
    public class OperationInfo
    {
        public OperationType OperationType { get; }

        public int FilesCount { get; }

        public int DirectoriesCount { get; }

        public string SourceDirectory { get; }

        public string TargetDirectory { get; }

        public OperationInfo(
            OperationType operationType,
            int filesCount,
            int directoriesCount,
            string sourceDirectory,
            string targetDirectory = null)
        {
            OperationType = operationType;
            FilesCount = filesCount;
            DirectoriesCount = directoriesCount;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
        }
    }
}