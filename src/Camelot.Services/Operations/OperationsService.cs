using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public class OperationsService : IOperationsService
    {
        private readonly IOperationsFactory _operationsFactory;
        private readonly IDirectoryService _directoryService;
        private readonly IResourceOpeningService _resourceOpeningService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly IFileOperationsStateService _fileOperationsStateService;

        public OperationsService(
            IOperationsFactory operationsFactory,
            IDirectoryService directoryService,
            IResourceOpeningService resourceOpeningService,
            IFileService fileService,
            IPathService pathService,
            IFileOperationsStateService fileOperationsStateService)
        {
            _operationsFactory = operationsFactory;
            _directoryService = directoryService;
            _resourceOpeningService = resourceOpeningService;
            _fileService = fileService;
            _pathService = pathService;
            _fileOperationsStateService = fileOperationsStateService;
        }

        public void OpenFiles(IReadOnlyList<string> files)
        {
            foreach (var selectedFile in files)
            {
                _resourceOpeningService.Open(selectedFile);
            }
        }

        public async Task CopyAsync(IReadOnlyList<string> nodes, string destinationDirectory)
        {
            var settings = GetBinaryFileSystemOperationSettings(nodes, destinationDirectory);
            var copyOperation = _operationsFactory.CreateCopyOperation(settings);
            _fileOperationsStateService.AddOperation(copyOperation);

            await copyOperation.RunAsync();
        }

        public async Task MoveAsync(IReadOnlyList<string> nodes, string destinationDirectory)
        {
            var settings = GetBinaryFileSystemOperationSettings(nodes, destinationDirectory);
            var moveOperation = _operationsFactory.CreateMoveOperation(settings);
            _fileOperationsStateService.AddOperation(moveOperation);

            await moveOperation.RunAsync();
        }

        public async Task MoveAsync(IReadOnlyDictionary<string, string> nodes)
        {
            var settings = GetBinaryFileSystemOperationSettings(nodes);
            var moveOperation = _operationsFactory.CreateMoveOperation(settings);
            _fileOperationsStateService.AddOperation(moveOperation);

            await moveOperation.RunAsync();
        }

        public async Task RemoveAsync(IReadOnlyList<string> nodes)
        {
            var (files, directories) = Split(nodes);
            var settings = Create(files, directories);
            var deleteOperation = _operationsFactory.CreateDeleteOperation(settings);
            _fileOperationsStateService.AddOperation(deleteOperation);

            await deleteOperation.RunAsync();
        }

        public void Rename(string path, string newName)
        {
            if (_fileService.CheckIfExists(path))
            {
                _fileService.Rename(path, newName);
            }
            else if (_directoryService.CheckIfExists(path))
            {
                _directoryService.Rename(path, newName);
            }
        }

        public void CreateDirectory(string sourceDirectory, string directoryName)
        {
            var fullPath = _pathService.Combine(sourceDirectory, directoryName);

            _directoryService.Create(fullPath);
        }

        private (string[] Files, string[] Directories) Split(IReadOnlyList<string> nodes)
        {
            var files = nodes
                .Where(_fileService.CheckIfExists)
                .ToArray();
            var directories = nodes
                .Where(_directoryService.CheckIfExists)
                .ToArray();

            return (files, directories);
        }

        private BinaryFileSystemOperationSettings GetBinaryFileSystemOperationSettings(
            IReadOnlyList<string> nodes,
            string outputDirectory)
        {
            var (files, directories) = Split(nodes);
            var sourceDirectory = GetCommonRootDirectory(nodes);
            var filesInDirectories = directories.SelectMany(_directoryService.GetFilesRecursively);
            var filePathsDictionary = filesInDirectories
                .Concat(files)
                .ToDictionary(
                    f => f,
                    f => GetDestinationPath(sourceDirectory, f, outputDirectory));
            var outputTopLevelFiles = files
                .Select(f => GetDestinationPath(sourceDirectory, f, outputDirectory))
                .ToArray();
            var outputTopLevelDirectories = directories
                .Select(f => GetDestinationPath(sourceDirectory, f, outputDirectory))
                .ToArray();

            return new BinaryFileSystemOperationSettings(directories, files, outputTopLevelDirectories,
                outputTopLevelFiles, filePathsDictionary);
        }

        private BinaryFileSystemOperationSettings GetBinaryFileSystemOperationSettings(
            IReadOnlyDictionary<string, string> nodes)
        {
            var (files, directories) = Split(nodes.Keys.ToArray());
            var outputTopLevelFiles = files
                .Select(f => nodes[f])
                .ToArray();
            var outputTopLevelDirectories = directories
                .Select(f => nodes[f])
                .ToArray();

            return new BinaryFileSystemOperationSettings(directories, files, outputTopLevelDirectories,
                outputTopLevelFiles, nodes);
        }

        private string GetCommonRootDirectory(IEnumerable<string> nodes) =>
            _pathService.GetCommonRootDirectory(nodes.ToArray());

        private string GetDestinationPath(string sourceDirectory,
            string sourcePath, string destinationDirectory)
        {
            var relativeSourcePath = _pathService.GetRelativePath(sourceDirectory, sourcePath);

            return _pathService.Combine(destinationDirectory, relativeSourcePath);
        }

        private UnaryFileSystemOperationSettings Create(
            IReadOnlyList<string> files, IReadOnlyList<string> directories) =>
            new UnaryFileSystemOperationSettings(directories, files);
    }
}