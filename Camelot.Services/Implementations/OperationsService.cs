using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Interfaces;
using Camelot.Services.Operations.Settings;

namespace Camelot.Services.Implementations
{
    public class OperationsService : IOperationsService
    {
        private readonly IFilesSelectionService _filesSelectionService;
        private readonly IOperationsFactory _operationsFactory;
        private readonly IDirectoryService _directoryService;
        private readonly IFileOpeningService _fileOpeningService;
        private readonly IFileService _fileService;

        public OperationsService(
            IFilesSelectionService filesSelectionService,
            IOperationsFactory operationsFactory,
            IDirectoryService directoryService,
            IFileOpeningService fileOpeningService,
            IFileService fileService)
        {
            _filesSelectionService = filesSelectionService;
            _operationsFactory = operationsFactory;
            _directoryService = directoryService;
            _fileOpeningService = fileOpeningService;
            _fileService = fileService;
        }

        public void EditSelectedFiles()
        {
            foreach (var selectedFile in _filesSelectionService.SelectedFiles)
            {
                _fileOpeningService.Open(selectedFile);
            }
        }

        public async Task CopySelectedFilesAsync(string destinationDirectory)
        {
            var files = GetBinaryFileOperationSettings(_directoryService.SelectedDirectory, destinationDirectory);
            if (!files.Any())
            {
                return;
            }

            var copyOperation = _operationsFactory.CreateCopyOperation(files);

            await copyOperation.RunAsync();
        }

        public async Task MoveSelectedFilesAsync(string destinationDirectory)
        {
            var files = GetBinaryFileOperationSettings(_directoryService.SelectedDirectory, destinationDirectory);
            if (!files.Any())
            {
                return;
            }

            var moveOperation = _operationsFactory.CreateMoveOperation(files);

            await moveOperation.RunAsync();
        }

        public async Task RemoveSelectedFilesAsync()
        {
            var files = GetUnaryFileOperationSettings();
            if (!files.Any())
            {
                return;
            }

            var deleteOperation = _operationsFactory.CreateDeleteFileOperation(files);

            await deleteOperation.RunAsync();
        }

        public void CreateDirectory(string directoryName)
        {
            var fullPath = Path.Combine(_directoryService.SelectedDirectory, directoryName);

            _directoryService.CreateDirectory(fullPath);
        }

        private UnaryFileOperationSettings[] GetUnaryFileOperationSettings()
        {
            var unaryFileOperationSettings = _filesSelectionService
                .SelectedFiles
                .Select(f => new UnaryFileOperationSettings(f))
                .ToArray();

            return unaryFileOperationSettings;
        }

        private BinaryFileOperationSettings[] GetBinaryFileOperationSettings(string sourceDirectory, string outputDirectory)
        {
            var files = _filesSelectionService
                .SelectedFiles
                .Where(_fileService.CheckIfFileExists);

            var filesInDirectories = _filesSelectionService
                .SelectedFiles
                .Where(_directoryService.CheckIfDirectoryExists)
                .SelectMany(_directoryService.GetFilesRecursively);

            var allFiles = files
                .Concat(filesInDirectories)
                .Select(sourcePath => Create(sourceDirectory, sourcePath, outputDirectory));

            return allFiles.ToArray();
        }

        private static BinaryFileOperationSettings Create(string sourceDirectory,
            string sourcePath, string destinationDirectory)
        {
            var relativeSourcePath = Path.GetRelativePath(sourceDirectory, sourcePath);
            var destinationPath = Path.Combine(destinationDirectory, relativeSourcePath);

            return new BinaryFileOperationSettings(sourcePath, destinationPath);
        }
    }
}