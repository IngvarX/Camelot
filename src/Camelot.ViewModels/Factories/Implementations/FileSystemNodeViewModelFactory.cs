using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
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
        private readonly IDialogService _dialogService;
        private readonly ITrashCanService _trashCanService;
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IArchiveService _archiveService;
        private readonly ISystemDialogService _systemDialogService;
        private readonly IOpenWithApplicationService _openWithApplicationService;

        public FileSystemNodeViewModelFactory(
            IFileSystemNodeOpeningBehavior fileOpeningBehavior,
            IFileSystemNodeOpeningBehavior directoryOpeningBehavior,
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService,
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesOperationsMediator filesOperationsMediator,
            IFileSystemNodePropertiesBehavior filePropertiesBehavior,
            IFileSystemNodePropertiesBehavior directoryPropertiesBehavior,
            IDialogService dialogService,
            ITrashCanService trashCanService,
            IFileService fileService,
            IDirectoryService directoryService,
            IArchiveService archiveService,
            ISystemDialogService systemDialogService, 
            IOpenWithApplicationService openWithApplicationService)
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
            _dialogService = dialogService;
            _trashCanService = trashCanService;
            _fileService = fileService;
            _directoryService = directoryService;
            _archiveService = archiveService;
            _systemDialogService = systemDialogService;
            _openWithApplicationService = openWithApplicationService;
        }

        public IFileSystemNodeViewModel Create(string path)
        {
            if (_fileService.CheckIfExists(path))
            {
                var fileModel = _fileService.GetFile(path);

                return fileModel is null ? null : Create(fileModel);
            }

            if (_directoryService.CheckIfExists(path))
            {
                var directoryModel = _directoryService.GetDirectory(path);

                return directoryModel is null ? null : Create(directoryModel, false);
            }

            return null;
        }

        public IFileSystemNodeViewModel Create(FileModel fileModel)
        {
            var fileViewModel = new FileViewModel(
                _fileOpeningBehavior,
                _operationsService,
                _clipboardOperationsService,
                _filesOperationsMediator,
                _fileSizeFormatter,
                _filePropertiesBehavior,
                _dialogService,
                _trashCanService,
                _archiveService,
                _systemDialogService,
                _openWithApplicationService,
                _pathService)
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
                _directoryPropertiesBehavior,
                _dialogService,
                _trashCanService,
                _archiveService,
                _systemDialogService,
                _openWithApplicationService,
                _pathService)
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