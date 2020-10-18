using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.Operations
{
    public class PackOperationSettings
    {
        public IReadOnlyList<string> InputTopLevelDirectories { get; }

        public IReadOnlyList<string> InputTopLevelFiles { get; }

        public string OutputTopLevelFile { get; }

        public string SourceDirectory { get; }

        public string TargetDirectory { get; }

        public ArchiveType ArchiveType { get; }

        public PackOperationSettings(
            IReadOnlyList<string> inputTopLevelDirectories,
            IReadOnlyList<string> inputTopLevelFiles,
            string outputTopLevelFile,
            string sourceDirectory,
            string targetDirectory,
            ArchiveType archiveType)
        {
            InputTopLevelDirectories = inputTopLevelDirectories;
            InputTopLevelFiles = inputTopLevelFiles;
            OutputTopLevelFile = outputTopLevelFile;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
            ArchiveType = archiveType;
        }
    }
}