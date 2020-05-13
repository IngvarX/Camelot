using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperationsFactory
    {
        IOperation CreateMoveOperation(BinaryFileSystemOperationSettings settings);

        IOperation CreateCopyOperation(BinaryFileSystemOperationSettings settings);

        IOperation CreateDeleteOperation(
            IReadOnlyList<string> topLevelDirectories,
            IReadOnlyList<string> topLevelFiles);

        IOperation CreateDeleteToTrashOperation(
            IReadOnlyList<string> topLevelDirectories,
            IReadOnlyList<string> topLevelFiles);
    }
}