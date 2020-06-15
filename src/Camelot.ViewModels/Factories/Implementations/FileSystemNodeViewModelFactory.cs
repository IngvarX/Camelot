using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class FileSystemNodeViewModelFactory : IFileSystemNodeViewModelFactory
    {
        private readonly IFileSystemNodeOpeningBehavior _fileOpeningBehavior;
        private readonly IFileSystemNodeOpeningBehavior _directoryOpeningBehavior;
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IPathService _pathService;
        private readonly IOperationsService _operationsService;
        private readonly IClipboardOperationsService _clipboardOperationsService;
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly IFileSystemNodePropertiesBehavior _filePropertiesBehavior;
        private readonly IFileSystemNodePropertiesBehavior _directoryPropertiesBehavior;

        public FileSystemNodeViewModelFactory(
            IFileSystemNodeOpeningBehavior fileOpeningBehavior,
            IFileSystemNodeOpeningBehavior directoryOpeningBehavior,
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService,
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesOperationsMediator filesOperationsMediator,
            IFileSystemNodePropertiesBehavior filePropertiesBehavior,
            IFileSystemNodePropertiesBehavior directoryPropertiesBehavior)
        {
            _fileOpeningBehavior = fileOpeningBehavior;
            _directoryOpeningBehavior = directoryOpeningBehavior;
            _fileSizeFormatter = fileSizeFormatter;
            _pathService = pathService;
            _operationsService = operationsService;
            _clipboardOperationsService = clipboardOperationsService;
            _filesOperationsMediator = filesOperationsMediator;
            _filePropertiesBehavior = filePropertiesBehavior;
            _directoryPropertiesBehavior = directoryPropertiesBehavior;
        }

        public IFileSystemNodeViewModel Create(FileModel fileModel)
        {
            var fileViewModel = new FileViewModel(
                _fileOpeningBehavior,
                _operationsService,
                _clipboardOperationsService,
                _filesOperationsMediator,
                _fileSizeFormatter,
                _filePropertiesBehavior)
            {
                FullPath = fileModel.FullPath,
                Size = fileModel.SizeBytes,
                LastModifiedDateTime = fileModel.LastModifiedDateTime,
                Name = _pathService.GetFileNameWithoutExtension(fileModel.Name),
                Extension = _pathService.GetExtension(fileModel.Name),
                FullName = _pathService.GetFileName(fileModel.Name)
            };

            return fileViewModel;
        }

        public IFileSystemNodeViewModel Create(DirectoryModel directoryModel, bool isParentDirectory)
        {
            var directoryViewModel = new DirectoryViewModel(
                _directoryOpeningBehavior,
                _operationsService,
                _clipboardOperationsService,
                _filesOperationsMediator,
                _directoryPropertiesBehavior)
            {
                FullPath = directoryModel.FullPath,
                Name = directoryModel.Name,
                LastModifiedDateTime = directoryModel.LastModifiedDateTime,
                FullName = directoryModel.Name,
                IsParentDirectory = isParentDirectory
            };

            return directoryViewModel;
        }
    }
}