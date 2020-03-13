using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Operations.Interfaces;
using Camelot.Services.Operations.Settings;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Services.Operations.Implementations
{
    public class OperationsFactory : IOperationsFactory
    {
        private readonly ITaskPool _taskPool;

        public OperationsFactory(ITaskPool taskPool)
        {
            _taskPool = taskPool;
        }

        public IOperation CreateMoveOperation(IList<BinaryFileOperationSettings> settings)
        {
            var operations = settings
                .Select(CreateMoveOperation)
                .ToArray();

            return CreateCompositeOperation(operations);
        }

        public IOperation CreateCopyOperation(IList<BinaryFileOperationSettings> settings)
        {
            var operations = settings
                .Select(CreateCopyOperation)
                .ToArray();

            return CreateCompositeOperation(operations);
        }

        public IOperation CreateDeleteOperation(IList<UnaryFileOperationSettings> files)
        {
            var operations = files
                .Select(CreateDeleteOperation)
                .ToArray();

            return CreateCompositeOperation(operations);
        }

        private static IOperation CreateMoveOperation(BinaryFileOperationSettings settings)
        {
            var copyOperation = CreateCopyOperation(settings);
            var deleteOperation = CreateDeleteOperation(settings.SourceFilePath);

            return new MoveOperation(copyOperation, deleteOperation);
        }

        private static IOperation CreateCopyOperation(BinaryFileOperationSettings settings)
        {
            return new CopyOperation(settings.SourceFilePath, settings.DestinationFilePath);
        }

        private static IOperation CreateDeleteOperation(UnaryFileOperationSettings settings)
        {
            return CreateDeleteOperation(settings.FilePath);
        }

        private static IOperation CreateDeleteOperation(string filePath)
        {
            return new RemovingOperation(filePath);
        }

        private IOperation CreateCompositeOperation(IList<IOperation> operations)
        {
            return new CompositeOperation(_taskPool, operations);
        }
    }
}