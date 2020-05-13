using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models.Operations
{
    public class OperationInfo
    {
        public OperationType OperationType { get; }

        public IReadOnlyList<string> Files { get; }

        public IReadOnlyList<string> Directories { get; }

        public string TargetDirectory { get; }

        public OperationInfo(
            OperationType operationType,
            IReadOnlyList<string> files,
            IReadOnlyList<string> directories,
            string targetDirectory = null)
        {
            OperationType = operationType;
            Files = files;
            Directories = directories;
            TargetDirectory = targetDirectory;
        }
    }
}