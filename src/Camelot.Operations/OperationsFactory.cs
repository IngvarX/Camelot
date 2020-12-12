using System.Collections.Generic;
using System.Linq;
using Camelot.Operations.Archive;
using Camelot.Operations.Models;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;
using Microsoft.Extensions.Logging;

namespace Camelot.Operations
{
    public class OperationsFactory : IOperationsFactory
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly IFileNameGenerationService _fileNameGenerationService;
        private readonly ILogger _logger;
        private readonly IArchiveProcessorFactory _archiveProcessorFactory;

        public OperationsFactory(
            IDirectoryService directoryService,
            IFileService fileService,
            IPathService pathService,
            IFileNameGenerationService fileNameGenerationService,
            ILogger logger,
            IArchiveProcessorFactory archiveProcessorFactory)
        {
            _directoryService = directoryService;
            _fileService = fileService;
            _pathService = pathService;
            _fileNameGenerationService = fileNameGenerationService;
            _logger = logger;
            _archiveProcessorFactory = archiveProcessorFactory;
        }

        public IOperation CreateCopyOperation(BinaryFileSystemOperationSettings settings)
        {
            var copyOperations = CreateCopyOperations(settings.FilesDictionary, settings.EmptyDirectories);
            var deleteNewFilesOperations = CreateDeleteOperations(settings.OutputTopLevelDirectories, settings.OutputTopLevelFiles);
            var operationGroup = CreateOperationGroup(copyOperations, deleteNewFilesOperations);

            var operations = CreateOperationsGroupsList(operationGroup);
            var operationInfo = CreateOperationInfo(OperationType.Copy, settings);

            var compositeOperation = CreateCompositeOperation(operations, operationInfo);

            return CreateOperation(compositeOperation);
        }

        public IOperation CreateMoveOperation(BinaryFileSystemOperationSettings settings)
        {
            var copyOperations = CreateCopyOperations(settings.FilesDictionary, settings.EmptyDirectories);
            var deleteNewFilesOperations = CreateDeleteOperations(settings.OutputTopLevelDirectories, settings.OutputTopLevelFiles);
            var copyOperationGroup = CreateOperationGroup(copyOperations, deleteNewFilesOperations);

            var deleteOldFilesOperations = CreateDeleteOperations(settings.InputTopLevelDirectories, settings.InputTopLevelFiles);
            var deleteOperationGroup = CreateOperationGroup(deleteOldFilesOperations);

            var operations = CreateOperationsGroupsList(copyOperationGroup, deleteOperationGroup);
            var operationInfo = CreateOperationInfo(OperationType.Move, settings);

            var compositeOperation = CreateCompositeOperation(operations, operationInfo);

            return CreateOperation(compositeOperation);
        }

        public IOperation CreateDeleteOperation(UnaryFileSystemOperationSettings settings)
        {
            var deleteOperations = CreateDeleteOperations(settings.TopLevelDirectories, settings.TopLevelFiles);
            var deleteOperationGroup = CreateOperationGroup(deleteOperations);

            var operations = CreateOperationsGroupsList(deleteOperationGroup);
            var operationInfo = CreateOperationInfo(OperationType.Delete, settings);

            var compositeOperation = CreateCompositeOperation(operations, operationInfo);

            return CreateOperation(compositeOperation);
        }

        public IOperation CreatePackOperation(PackOperationSettings settings)
        {
            var archiveWriter = CreateArchiveWriter(settings.ArchiveType);
            var packOperation = CreatePackOperation(archiveWriter, settings);
            var operationGroup = CreateOperationGroup(new[] {packOperation});
            var operations = CreateOperationsGroupsList(operationGroup);
            var operationInfo = CreateOperationInfo(settings);

            var compositeOperation = CreateCompositeOperation(operations, operationInfo);

            return CreateOperation(compositeOperation);
        }

        public IOperation CreateExtractOperation(ExtractArchiveOperationSettings settings)
        {
            var archiveProcessor = CreateArchiveReader(settings.ArchiveType);
            var extractOperation = CreateExtractOperation(archiveProcessor, settings.InputTopLevelFile, settings.TargetDirectory);
            var operationGroup = CreateOperationGroup(new[] {extractOperation});
            var operations = CreateOperationsGroupsList(operationGroup);
            var operationInfo = CreateOperationInfo(settings);

            var compositeOperation = CreateCompositeOperation(operations, operationInfo);

            return CreateOperation(compositeOperation);
        }

        private IInternalOperation[] CreateCopyOperations(
            IReadOnlyDictionary<string, string> filesDictionary,
            IReadOnlyList<string> emptyDirectoriesDictionary)
        {
            var filesOperations = filesDictionary
                .Select(kvp => CreateCopyOperation(kvp.Key, kvp.Value));
            var directoriesOperations = emptyDirectoriesDictionary
                .Select(CreateAddDirectoryOperation);

            return filesOperations.Concat(directoriesOperations).ToArray();
        }

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
            new DeleteFileOperation(_fileService, filePath);

        private IInternalOperation CreateDeleteDirectoryOperation(string directoryPath) =>
            new DeleteDirectoryOperation(_directoryService, directoryPath);

        private IInternalOperation CreateAddDirectoryOperation(string directoryPath) =>
            new CreateDirectoryOperation(_directoryService, directoryPath);

        private IInternalOperation CreatePackOperation(IArchiveWriter archiveWriter,
            PackOperationSettings settings) =>
            new PackOperation(archiveWriter, _directoryService, _pathService, settings);

        private IInternalOperation CreateExtractOperation(IArchiveReader archiveReader,
            string archiveFilePath, string outputDirectory) =>
            new ExtractOperation(archiveReader, _directoryService, archiveFilePath, outputDirectory);

        private IArchiveReader CreateArchiveReader(ArchiveType archiveType) =>
            _archiveProcessorFactory.CreateReader(archiveType);

        private IArchiveWriter CreateArchiveWriter(ArchiveType archiveType) =>
            _archiveProcessorFactory.CreateWriter(archiveType);

        private ICompositeOperation CreateCompositeOperation(
            IReadOnlyList<OperationGroup> operations,
            OperationInfo operationInfo) =>
            new CompositeOperation(_fileNameGenerationService, operations, operationInfo);

        private IOperation CreateOperation(ICompositeOperation compositeOperation) =>
            new AsyncOperationStateMachine(compositeOperation, _logger);

        private static OperationInfo CreateOperationInfo(OperationType operationType, BinaryFileSystemOperationSettings settings) =>
            new OperationInfo(operationType, settings);

        private static OperationInfo CreateOperationInfo(OperationType operationType, UnaryFileSystemOperationSettings settings) =>
            new OperationInfo(operationType, settings);

        private static OperationInfo CreateOperationInfo(PackOperationSettings settings) =>
            new OperationInfo(settings);

        private static OperationInfo CreateOperationInfo(ExtractArchiveOperationSettings settings) =>
            new OperationInfo(settings);

        private static IReadOnlyList<OperationGroup> CreateOperationsGroupsList(
            params OperationGroup[] operations) => operations;

        private static OperationGroup CreateOperationGroup(
            IReadOnlyList<IInternalOperation> operations, IReadOnlyList<IInternalOperation> cancelOperations = null) =>
            new OperationGroup(operations, cancelOperations);
    }
}