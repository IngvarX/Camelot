using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Services.Operations
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

        public IOperation CreateDeleteOperation(
            IList<UnaryFileOperationSettings> directories,
            IList<UnaryFileOperationSettings> files)
        {
            var fileOperations = files
                .Select(CreateDeleteFileOperation);
            var directoryOperations = directories
                .Select(CreateDeleteDirectoryOperation);
            var operations = fileOperations.Concat(directoryOperations).ToArray();

            return CreateCompositeOperation(operations);
        }

        public IOperation CreateDeleteToTrashOperation(IList<UnaryFileOperationSettings> parameters)
        {
            return ?
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

        private IInternalOperation CreateCopyOperation(BinaryFileOperationSettings settings) =>
            new CopyOperation(_directoryService, _fileService, _pathService,
                settings.SourceFilePath, settings.DestinationFilePath);

        private IInternalOperation CreateDeleteFileOperation(UnaryFileOperationSettings settings) =>
            CreateDeleteFileOperation(settings.FilePath);

        private IInternalOperation CreateDeleteDirectoryOperation(UnaryFileOperationSettings settings) =>
            CreateDeleteDirectoryOperation(settings.FilePath);

        private IInternalOperation CreateDeleteFileOperation(string filePath) =>
            new RemoveFileOperation(filePath, _fileService);

        private IInternalOperation CreateDeleteDirectoryOperation(string filePath) =>
            new RemoveDirectoryOperation(filePath, _directoryService);

        private IOperation CreateCompositeOperation(IList<IInternalOperation> operations) =>
            new CompositeOperation(_taskPool, operations);
    }
}