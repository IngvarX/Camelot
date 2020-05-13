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

        public OperationsService(
            IOperationsFactory operationsFactory,
            IDirectoryService directoryService,
            IResourceOpeningService resourceOpeningService,
            IFileService fileService,
            IPathService pathService)
        {
            _operationsFactory = operationsFactory;
            _directoryService = directoryService;
            _resourceOpeningService = resourceOpeningService;
            _fileService = fileService;
            _pathService = pathService;
        }

        public void OpenFiles(IReadOnlyCollection<string> files)
        {
            foreach (var selectedFile in files)
            {
                _resourceOpeningService.Open(selectedFile);
            }
        }

        public async Task CopyAsync(IReadOnlyCollection<string> nodes, string destinationDirectory)
        {
            var settings = GetBinaryFileSystemOperationSettings(nodes, destinationDirectory);
            var copyOperation = _operationsFactory.CreateCopyOperation(settings);

            await copyOperation.RunAsync();
        }

        public async Task MoveAsync(IReadOnlyCollection<string> nodes, string destinationDirectory)
        {
            var filesSettings = GetBinaryFileSystemOperationSettings(nodes, destinationDirectory);
            var moveOperation = _operationsFactory.CreateMoveOperation(filesSettings);

            await moveOperation.RunAsync();
        }

        public async Task MoveAsync(IDictionary<string, string> nodes)
        {
            // var filesSettings = GetBinaryFileOperationSettings(nodes);
            // if (!filesSettings.Any())
            // {
            //     return;
            // }
            //
            // var moveOperation = _operationsFactory.CreateMoveOperation(filesSettings);
            //
            // await moveOperation.RunAsync();
        }

        public async Task RemoveAsync(IReadOnlyCollection<string> files)
        {
            var (filesSettings, directoriesSettings) = Split(files);
            var deleteOperation =
                _operationsFactory.CreateDeleteOperation(filesSettings, directoriesSettings);

            await deleteOperation.RunAsync();
        }

        public async Task RemoveToTrashAsync(IReadOnlyCollection<string> files)
        {
            var (filesSettings, directoriesSettings) = Split(files);
            var deleteToTrashOperation =
                _operationsFactory.CreateDeleteToTrashOperation(filesSettings, directoriesSettings);

            await deleteToTrashOperation.RunAsync();
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

        private (string[] Files, string[] Directories) Split(IReadOnlyCollection<string> nodes)
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
            IReadOnlyCollection<string> nodes,
            string outputDirectory)
        {
            var (files, directories) = Split(nodes);
            var sourceDirectory = GetCommonRootDirectory(files) ?? GetCommonRootDirectory(directories);
            var filesInDirectories = directories.SelectMany(_directoryService.GetFilesRecursively);
            var filePathsDictionary = filesInDirectories.ToDictionary(
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

        // private BinaryFileOperationSettings GetBinaryFileOperationSettings(
        //     IDictionary<string, string> selectedFiles)
        // {
        //     var files = selectedFiles.Keys
        //         .Where(_fileService.CheckIfExists)
        //         .Select(sourcePath => Create(sourcePath, selectedFiles[sourcePath]));
        //
        //     var filesInDirectories = selectedFiles.Keys
        //         .Where(_directoryService.CheckIfExists)
        //         .Select(d => new {Dir = d, Files = _directoryService.GetFilesRecursively(d)})
        //         .SelectMany(info => info.Files.Select(f =>
        //             Create(f, _pathService.Combine(selectedFiles[info.Dir], f.Substring(info.Dir.Length + 1)))));
        //
        //     return files.Concat(filesInDirectories).ToArray();
        // }

        private string GetCommonRootDirectory(IEnumerable<string> nodes) =>
            _pathService.GetCommonRootDirectory(nodes.ToArray());

        private string GetDestinationPath(string sourceDirectory,
            string sourcePath, string destinationDirectory)
        {
            var relativeSourcePath = _pathService.GetRelativePath(sourceDirectory, sourcePath);

            return _pathService.Combine(destinationDirectory, relativeSourcePath);
        }
    }
}