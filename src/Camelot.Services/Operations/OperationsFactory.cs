using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
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

        public IOperation CreateCopyOperation(BinaryFileSystemOperationSettings settings)
        {
            var copyOperations = CreateCopyOperations(settings.FilesDictionary);
            var operations = CreateOperationsGroupsList(copyOperations);

            var deleteNewFilesOperations = CreateDeleteOperations(settings.OutputTopLevelDirectories, settings.OutputTopLevelFiles);
            var cancelOperations = CreateOperationsGroupsList(deleteNewFilesOperations);

            var operationInfo = Create(OperationType.Copy, settings.InputTopLevelDirectories, settings.InputTopLevelFiles);

            return CreateCompositeOperation(operations, cancelOperations, operationInfo);
        }

        public IOperation CreateMoveOperation(BinaryFileSystemOperationSettings settings)
        {
            var copyOperations = CreateCopyOperations(settings.FilesDictionary);
            var deleteOldFilesOperations = CreateDeleteOperations(settings.InputTopLevelDirectories, settings.InputTopLevelFiles);
            var operations = CreateOperationsGroupsList(copyOperations, deleteOldFilesOperations);

            var deleteNewFilesOperations = CreateDeleteOperations(settings.OutputTopLevelDirectories, settings.OutputTopLevelFiles);
            var cancelOperations = CreateOperationsGroupsList(deleteNewFilesOperations);

            var operationInfo = Create(OperationType.Move, settings.InputTopLevelDirectories, settings.InputTopLevelFiles);

            return CreateCompositeOperation(operations, cancelOperations, operationInfo);
        }

        public IOperation CreateDeleteOperation(UnaryFileSystemOperationSettings settings)
        {
            var deleteOperations = CreateDeleteOperations(settings.TopLevelDirectories, settings.TopLevelFiles);
            var operations = CreateOperationsGroupsList(deleteOperations);

            var cancelOperations = CreateOperationsGroupsList();

            var operationInfo = Create(OperationType.Delete, settings.TopLevelFiles, settings.TopLevelDirectories);

            return CreateCompositeOperation(operations, cancelOperations, operationInfo);
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

        private IOperation CreateCompositeOperation(
            IReadOnlyList<IReadOnlyList<IInternalOperation>> operations,
            IReadOnlyList<IReadOnlyList<IInternalOperation>> cancelOperations,
            OperationInfo operationInfo) =>
            new CompositeOperation(_taskPool, operations, cancelOperations, operationInfo);

        private static OperationInfo Create(OperationType operationType,
            IReadOnlyList<string> directories, IReadOnlyList<string> files) =>
            new OperationInfo(operationType, files, directories);

        private static IReadOnlyList<IReadOnlyList<IInternalOperation>> CreateOperationsGroupsList(
            params IReadOnlyList<IInternalOperation>[] operations) => operations;
    }
}