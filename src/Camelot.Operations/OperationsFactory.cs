using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Camelot.TaskPool.Interfaces;

namespace Camelot.Operations
{
    public class OperationsFactory : IOperationsFactory
    {
        private readonly ITaskPool _taskPool;
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly IFileNameGenerationService _fileNameGenerationService;

        public OperationsFactory(
            ITaskPool taskPool,
            IDirectoryService directoryService,
            IFileService fileService,
            IPathService pathService,
            IFileNameGenerationService fileNameGenerationService)
        {
            _taskPool = taskPool;
            _directoryService = directoryService;
            _fileService = fileService;
            _pathService = pathService;
            _fileNameGenerationService = fileNameGenerationService;
        }

        public IOperation CreateCopyOperation(BinaryFileSystemOperationSettings settings)
        {
            var copyOperations = CreateCopyOperations(settings.FilesDictionary);
            var deleteNewFilesOperations = CreateDeleteOperations(settings.OutputTopLevelDirectories, settings.OutputTopLevelFiles);
            var operationGroup = CreateOperationGroup(copyOperations, deleteNewFilesOperations);

            var operations = CreateOperationsGroupsList(operationGroup);
            var operationInfo = Create(OperationType.Copy, settings);

            var compositeOperation = CreateCompositeOperation(operations, operationInfo);

            return CreateOperation(compositeOperation);
        }

        public IOperation CreateMoveOperation(BinaryFileSystemOperationSettings settings)
        {
            var copyOperations = CreateCopyOperations(settings.FilesDictionary);
            var deleteNewFilesOperations = CreateDeleteOperations(settings.OutputTopLevelDirectories, settings.OutputTopLevelFiles);
            var copyOperationGroup = CreateOperationGroup(copyOperations, deleteNewFilesOperations);

            var deleteOldFilesOperations = CreateDeleteOperations(settings.InputTopLevelDirectories, settings.InputTopLevelFiles);
            var deleteOperationGroup = CreateOperationGroup(deleteOldFilesOperations);

            var operations = CreateOperationsGroupsList(copyOperationGroup, deleteOperationGroup);
            var operationInfo = Create(OperationType.Move, settings);

            var compositeOperation = CreateCompositeOperation(operations, operationInfo);

            return CreateOperation(compositeOperation);
        }

        public IOperation CreateDeleteOperation(UnaryFileSystemOperationSettings settings)
        {
            var deleteOperations = CreateDeleteOperations(settings.TopLevelDirectories, settings.TopLevelFiles);
            var deleteOperationGroup = CreateOperationGroup(deleteOperations);

            var operations = CreateOperationsGroupsList(deleteOperationGroup);
            var operationInfo = Create(OperationType.Delete,settings);

            var compositeOperation = CreateCompositeOperation(operations, operationInfo);

            return CreateOperation(compositeOperation);
        }

        private IInternalOperation[] CreateCopyOperations(IReadOnlyDictionary<string, string> filesDictionary) =>
            filesDictionary
                .Select(kvp => CreateCopyOperation(kvp.Key, kvp.Value))
                .ToArray();

        private IInternalOperation[] CreateDeleteOperations(
            IReadOnlyList<string> topLevelDirectories,
            IReadOnlyList<string> topLevelFiles)
        {
            var fileOperations = topLevelFiles
                .Select(CreateDeleteFileOperation);
            var directoryOperations = topLevelDirectories
                .Select(CreateDeleteDirectoryOperation);

            return fileOperations.Concat(directoryOperations).ToArray();
        }

        private IInternalOperation CreateCopyOperation(string source, string destination) =>
            new CopyOperation(_directoryService, _fileService, _pathService, source, destination);

        private IInternalOperation CreateDeleteFileOperation(string filePath) =>
            new DeleteFileOperation(filePath, _fileService);

        private IInternalOperation CreateDeleteDirectoryOperation(string filePath) =>
            new DeleteDirectoryOperation(filePath, _directoryService);

        private ICompositeOperation CreateCompositeOperation(
            IReadOnlyList<OperationGroup> operations,
            OperationInfo operationInfo) =>
            new CompositeOperation(_taskPool, _fileNameGenerationService, operations, operationInfo);

        private static IOperation CreateOperation(ICompositeOperation compositeOperation) =>
            new AsyncOperationStateMachine(compositeOperation);

        private static OperationInfo Create(OperationType operationType, BinaryFileSystemOperationSettings settings) =>
            new OperationInfo(operationType, settings);

        private static OperationInfo Create(OperationType operationType, UnaryFileSystemOperationSettings settings) =>
            new OperationInfo(operationType, settings);

        private static IReadOnlyList<OperationGroup> CreateOperationsGroupsList(
            params OperationGroup[] operations) => operations;

        private static OperationGroup CreateOperationGroup(
            IInternalOperation[] operations, IInternalOperation[] cancelOperations = null) =>
            new OperationGroup(operations, cancelOperations);
    }
}