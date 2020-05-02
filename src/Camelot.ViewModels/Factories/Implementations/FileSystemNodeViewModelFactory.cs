using System.Globalization;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Models;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
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

        public FileSystemNodeViewModelFactory(
            IFileSystemNodeOpeningBehavior fileOpeningBehavior,
            IFileSystemNodeOpeningBehavior directoryOpeningBehavior,
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService,
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesOperationsMediator filesOperationsMediator)
        {
            _fileOpeningBehavior = fileOpeningBehavior;
            _directoryOpeningBehavior = directoryOpeningBehavior;
            _fileSizeFormatter = fileSizeFormatter;
            _pathService = pathService;
            _operationsService = operationsService;
            _clipboardOperationsService = clipboardOperationsService;
            _filesOperationsMediator = filesOperationsMediator;
        }

        public IFileSystemNodeViewModel Create(FileModel fileModel)
        {
            var fileViewModel = new FileViewModel(
                _fileOpeningBehavior,
                _operationsService,
                _clipboardOperationsService,
                _filesOperationsMediator,
                _fileSizeFormatter)
            {
                FullPath = fileModel.FullPath,
                Size = fileModel.SizeBytes,
                LastModifiedDateTime = fileModel.LastWriteTime.ToString(CultureInfo.CurrentCulture),
                Name = _pathService.GetFileNameWithoutExtension(fileModel.Name),
                Extension = _pathService.GetExtension(fileModel.Name),
                FullName = _pathService.GetFileName(fileModel.Name)
            };

            return fileViewModel;
        }

        public IFileSystemNodeViewModel Create(DirectoryModel directoryModel, bool isParentDirectory)
        {
            var fileViewModel = new DirectoryViewModel(
                _directoryOpeningBehavior,
                _operationsService,
                _clipboardOperationsService,
                _filesOperationsMediator)
            {
                FullPath = directoryModel.FullPath,
                Name = directoryModel.Name,
                LastModifiedDateTime = directoryModel.LastModifiedDateTime.ToString(CultureInfo.CurrentCulture),
                FullName = directoryModel.Name,
                IsParentDirectory = isParentDirectory
            };

            return fileViewModel;
        }
    }
}