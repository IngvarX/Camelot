using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperationsFactory
    {
        IOperation CreateCopyOperation(BinaryFileSystemOperationSettings settings);

        IOperation CreateMoveOperation(BinaryFileSystemOperationSettings settings);

        IOperation CreateDeleteOperation(UnaryFileSystemOperationSettings settings);

        IOperation CreatePackOperation(PackOperationSettings settings);

        IOperation CreateExtractOperation(ExtractArchiveOperationSettings settings);
    }
}