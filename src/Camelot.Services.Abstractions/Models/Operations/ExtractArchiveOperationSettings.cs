using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.Operations
{
    public class ExtractArchiveOperationSettings
    {
        public string InputTopLevelFile { get; }

        public string TargetDirectory { get; }

        public ArchiveType ArchiveType { get; }

        public ExtractArchiveOperationSettings(
            string inputTopLevelFile,
            string targetDirectory,
            ArchiveType archiveType)
        {
            InputTopLevelFile = inputTopLevelFile;
            TargetDirectory = targetDirectory;
            ArchiveType = archiveType;
        }
    }
}