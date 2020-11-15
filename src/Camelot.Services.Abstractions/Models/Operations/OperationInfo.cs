using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.Operations
{
    public class OperationInfo
    {
        public OperationType OperationType { get; }

        public IReadOnlyList<string> Files { get; }

        public IReadOnlyList<string> Directories { get; }

        public int TotalFilesCount { get; }

        public string SourceDirectory { get; }

        public string TargetDirectory { get; }

        public OperationInfo(OperationType operationType, BinaryFileSystemOperationSettings settings)
        {
            OperationType = operationType;
            Files = settings.InputTopLevelFiles;
            Directories = settings.InputTopLevelDirectories;
            TotalFilesCount = settings.FilesDictionary.Count;
            SourceDirectory = settings.SourceDirectory;
            TargetDirectory = settings.TargetDirectory;
        }

        public OperationInfo(OperationType operationType, UnaryFileSystemOperationSettings settings)
        {
            OperationType = operationType;
            Files = settings.TopLevelFiles;
            Directories = settings.TopLevelDirectories;
            TotalFilesCount = Files.Count + Directories.Count;
            SourceDirectory = settings.SourceDirectory;
        }

        public OperationInfo(ExtractArchiveOperationSettings settings)
        {
            OperationType = OperationType.Extract;
            Files = new[] {settings.InputTopLevelFile};
            Directories = Array.Empty<string>();
            TotalFilesCount = 1;
            TargetDirectory = settings.TargetDirectory;
        }

        public OperationInfo(PackOperationSettings settings)
        {
            OperationType = OperationType.Pack;
            Files = settings.InputTopLevelFiles;
            Directories = settings.InputTopLevelDirectories;
            TotalFilesCount = Files.Count;
            TargetDirectory = settings.TargetDirectory;
        }
    }
}