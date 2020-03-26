using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Interfaces;
using Camelot.Services.Operations.Settings;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Services.Operations.Implementations
{
    public class OperationsFactory : IOperationsFactory
    {
        private readonly ITaskPool _taskPool;
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;

        public OperationsFactory(
            ITaskPool taskPool,
            IDirectoryService directoryService,
            IFileService fileService,
            IPathService pathService)
        {
            _taskPool = taskPool;
            _directoryService = directoryService;
            _fileService = fileService;
            _pathService = pathService;
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

        public IOperation CreateDeleteFileOperation(IList<UnaryFileOperationSettings> files)
        {
            var operations = files
                .Select(CreateDeleteFileOperation)
                .ToArray();

            return CreateCompositeOperation(operations);
        }

        public IOperation CreateDeleteDirectoryOperation(IList<UnaryFileOperationSettings> directories)
        {
            var operations = directories
                .Select(CreateDeleteDirectoryOperation)
                .ToArray();

            return CreateCompositeOperation(operations);
        }

        private IOperation CreateMoveOperation(BinaryFileOperationSettings settings)
        {
            var copyOperation = CreateCopyOperation(settings);
            // TODO: cleanup folders
            var deleteOperation = CreateDeleteFileOperation(settings.SourceFilePath);

            return new MoveOperation(copyOperation, deleteOperation);
        }

        private IOperation CreateCopyOperation(BinaryFileOperationSettings settings)
        {
            return new CopyOperation(_directoryService,
                _fileService, _pathService,
                settings.SourceFilePath, settings.DestinationFilePath);
        }

        private IOperation CreateDeleteFileOperation(UnaryFileOperationSettings settings)
        {
            return CreateDeleteFileOperation(settings.FilePath);
        }

        private IOperation CreateDeleteDirectoryOperation(UnaryFileOperationSettings settings)
        {
            return CreateDeleteDirectoryOperation(settings.FilePath);
        }

        private IOperation CreateDeleteFileOperation(string filePath)
        {
            return new RemovingFileOperation(filePath, _fileService);
        }

        private IOperation CreateDeleteDirectoryOperation(string filePath)
        {
            return new RemovingDirectoryOperation(filePath, _directoryService);
        }

        private IOperation CreateCompositeOperation(IList<IOperation> operations)
        {
            return new CompositeOperation(_taskPool, operations);
        }
    }
}