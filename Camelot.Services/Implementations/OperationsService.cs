using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Interfaces;
using Camelot.Services.Operations.Settings;

namespace Camelot.Services.Implementations
{
    public class OperationsService : IOperationsService
    {
        private readonly IOperationsFactory _operationsFactory;
        private readonly IDirectoryService _directoryService;
        private readonly IFileOpeningService _fileOpeningService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;

        public OperationsService(
            IOperationsFactory operationsFactory,
            IDirectoryService directoryService,
            IFileOpeningService fileOpeningService,
            IFileService fileService,
            IPathService pathService)
        {
            _operationsFactory = operationsFactory;
            _directoryService = directoryService;
            _fileOpeningService = fileOpeningService;
            _fileService = fileService;
            _pathService = pathService;
        }

        public void EditFiles(IReadOnlyCollection<string> files)
        {
            foreach (var selectedFile in files)
            {
                _fileOpeningService.Open(selectedFile);
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

        public void CreateDirectory(string sourceDirectory, string directoryName)
        {
            var fullPath = _pathService.Combine(sourceDirectory, directoryName);

            _directoryService.CreateDirectory(fullPath);
        }

        private (UnaryFileOperationSettings[], UnaryFileOperationSettings[]) GetUnaryFileOperationSettings(
            IReadOnlyCollection<string> files)
        {
            var unaryFileOperationSettings = files
                .Where(_fileService.CheckIfFileExists)
                .Select(f => new UnaryFileOperationSettings(f))
                .ToArray();
            var unaryDirectoryOperationSettings = files
                .Where(_directoryService.CheckIfDirectoryExists)
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
                .Where(_fileService.CheckIfFileExists);

            var filesInDirectories = selectedFiles
                .Where(_directoryService.CheckIfDirectoryExists)
                .SelectMany(_directoryService.GetFilesRecursively);

            var allFiles = files
                .Concat(filesInDirectories)
                .Select(sourcePath => Create(sourceDirectory, sourcePath, outputDirectory));

            return allFiles.ToArray();
        }

        private string GetCommonRootDirectory(IEnumerable<string> files)
        {
            return _pathService.GetCommonRootDirectory(files.ToArray());
        }

        private BinaryFileOperationSettings Create(string sourceDirectory,
            string sourcePath, string destinationDirectory)
        {
            var relativeSourcePath = _pathService.GetRelativePath(sourceDirectory, sourcePath);
            var destinationPath = _pathService.Combine(destinationDirectory, relativeSourcePath);

            return new BinaryFileOperationSettings(sourcePath, destinationPath);
        }
    }
}