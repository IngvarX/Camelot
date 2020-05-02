using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Models.Operations;

namespace Camelot.Services
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

        public async Task CopyFilesAsync(IReadOnlyCollection<string> files, string destinationDirectory)
        {
            var filesSettings = GetBinaryFileOperationSettings(files, destinationDirectory);
            if (!filesSettings.Any())
            {
                return;
            }

            var copyOperation = _operationsFactory.CreateCopyOperation(filesSettings);

            await copyOperation.RunAsync();
        }

        public async Task MoveFilesAsync(IReadOnlyCollection<string> files, string destinationDirectory)
        {
            var filesSettings = GetBinaryFileOperationSettings(files, destinationDirectory);
            if (!filesSettings.Any())
            {
                return;
            }

            var moveOperation = _operationsFactory.CreateMoveOperation(filesSettings);

            await moveOperation.RunAsync();
        }

        public async Task MoveFilesAsync(IDictionary<string, string> files)
        {
            var filesSettings = GetBinaryFileOperationSettings(files);
            if (!filesSettings.Any())
            {
                return;
            }

            var moveOperation = _operationsFactory.CreateMoveOperation(filesSettings);

            await moveOperation.RunAsync();
        }

        public async Task RemoveFilesAsync(IReadOnlyCollection<string> files)
        {
            var (filesSettings, directoriesSettings) = GetUnaryFileOperationSettings(files);
            if (filesSettings.Any())
            {
                var deleteFilesOperation = _operationsFactory.CreateDeleteFileOperation(filesSettings);

                await deleteFilesOperation.RunAsync();
            }

            if (directoriesSettings.Any())
            {
                var deleteDirectoriesOperation = _operationsFactory.CreateDeleteDirectoryOperation(directoriesSettings);

                await deleteDirectoriesOperation.RunAsync();
            }
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

        private (UnaryFileOperationSettings[], UnaryFileOperationSettings[]) GetUnaryFileOperationSettings(
            IReadOnlyCollection<string> files)
        {
            var unaryFileOperationSettings = files
                .Where(_fileService.CheckIfExists)
                .Select(f => new UnaryFileOperationSettings(f))
                .ToArray();
            var unaryDirectoryOperationSettings = files
                .Where(_directoryService.CheckIfExists)
                .Select(f => new UnaryFileOperationSettings(f))
                .ToArray();

            return (unaryFileOperationSettings, unaryDirectoryOperationSettings);
        }

        private BinaryFileOperationSettings[] GetBinaryFileOperationSettings(
            IReadOnlyCollection<string> selectedFiles,
            string outputDirectory)
        {
            var sourceDirectory = GetCommonRootDirectory(selectedFiles);
            var files = selectedFiles
                .Where(_fileService.CheckIfExists);

            var filesInDirectories = selectedFiles
                .Where(_directoryService.CheckIfExists)
                .SelectMany(_directoryService.GetFilesRecursively);

            var allFiles = files
                .Concat(filesInDirectories)
                .Select(sourcePath => Create(sourceDirectory, sourcePath, outputDirectory));

            return allFiles.ToArray();
        }
        
        private BinaryFileOperationSettings[] GetBinaryFileOperationSettings(
            IDictionary<string, string> selectedFiles)
        {
            var files = selectedFiles.Keys
                .Where(_fileService.CheckIfExists)
                .Select(sourcePath => Create(sourcePath, selectedFiles[sourcePath]));

            var filesInDirectories = selectedFiles.Keys
                .Where(_directoryService.CheckIfExists)
                .Select(d => new {Dir = d, Files = _directoryService.GetFilesRecursively(d)})
                .SelectMany(info => info.Files.Select(f =>
                    Create(f, _pathService.Combine(selectedFiles[info.Dir], f.Substring(info.Dir.Length + 1)))));
            
            return files.Concat(filesInDirectories).ToArray();
        }
        
        private string GetCommonRootDirectory(IEnumerable<string> files) =>
            _pathService.GetCommonRootDirectory(files.ToArray());

        private BinaryFileOperationSettings Create(string sourceDirectory,
            string sourcePath, string destinationDirectory)
        {
            var relativeSourcePath = _pathService.GetRelativePath(sourceDirectory, sourcePath);
            var destinationPath = _pathService.Combine(destinationDirectory, relativeSourcePath);

            return Create(sourcePath, destinationPath);
        }

        private static BinaryFileOperationSettings Create(string sourcePath, string destinationPath)
            => new BinaryFileOperationSettings(sourcePath, destinationPath);
    }
}