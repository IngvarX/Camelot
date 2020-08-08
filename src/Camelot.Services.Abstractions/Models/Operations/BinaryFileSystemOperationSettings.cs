using System.Collections.Generic;

namespace Camelot.Services.Abstractions.Models.Operations
{
    public class BinaryFileSystemOperationSettings
    {
        public IReadOnlyList<string> InputTopLevelDirectories { get; }

        public IReadOnlyList<string> InputTopLevelFiles { get; }

        public IReadOnlyList<string> OutputTopLevelDirectories { get; }

        public IReadOnlyList<string> OutputTopLevelFiles { get; }

        public IReadOnlyDictionary<string, string> FilesDictionary { get; }

        public IReadOnlyList<string> EmptyDirectories { get; }

        public string SourceDirectory { get; }

        public string TargetDirectory { get; }

        public BinaryFileSystemOperationSettings(
            IReadOnlyList<string> inputTopLevelDirectories,
            IReadOnlyList<string> inputTopLevelFiles,
            IReadOnlyList<string> outputTopLevelDirectories,
            IReadOnlyList<string> outputTopLevelFiles,
            IReadOnlyDictionary<string, string> filesDictionary,
            IReadOnlyList<string> emptyDirectories,
            string sourceDirectory = null,
            string targetDirectory = null)
        {
            InputTopLevelDirectories = inputTopLevelDirectories;
            InputTopLevelFiles = inputTopLevelFiles;
            OutputTopLevelDirectories = outputTopLevelDirectories;
            OutputTopLevelFiles = outputTopLevelFiles;
            FilesDictionary = filesDictionary;
            EmptyDirectories = emptyDirectories;
            SourceDirectory = sourceDirectory;
            TargetDirectory = targetDirectory;
        }
    }
}