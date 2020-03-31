using System.Globalization;
using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class FileSystemNodeViewModelFactory : IFileSystemNodeViewModelFactory
    {
        private readonly IFileSystemNodeOpeningBehavior _fileSystemNodeOpeningBehavior;
        private readonly IFileSystemNodeOpeningBehavior _directorySystemNodeOpeningBehavior;
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IPathService _pathService;
        private readonly IOperationsService _operationsService;

        public FileSystemNodeViewModelFactory(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IFileSystemNodeOpeningBehavior directorySystemNodeOpeningBehavior,
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService,
            IOperationsService operationsService)
        {
            _fileSystemNodeOpeningBehavior = fileSystemNodeOpeningBehavior;
            _directorySystemNodeOpeningBehavior = directorySystemNodeOpeningBehavior;
            _fileSizeFormatter = fileSizeFormatter;
            _pathService = pathService;
            _operationsService = operationsService;
        }

        public IFileSystemNodeViewModel Create(FileModel fileModel)
        {
            var fileViewModel = new FileViewModel(_fileSystemNodeOpeningBehavior, _operationsService)
            {
                FullPath = fileModel.FullPath,
                Size = _fileSizeFormatter.GetFormattedSize(fileModel.SizeBytes),
                LastModifiedDateTime = fileModel.LastWriteTime.ToString(CultureInfo.CurrentCulture),
                Name = _pathService.GetFileNameWithoutExtension(fileModel.Name),
                Extension = _pathService.GetExtension(fileModel.Name)
            };

            return fileViewModel;
        }

        public IFileSystemNodeViewModel Create(DirectoryModel directoryModel)
        {
            var fileViewModel = new DirectoryViewModel(_directorySystemNodeOpeningBehavior, _operationsService)
            {
                FullPath = directoryModel.FullPath,
                Name = _pathService.GetFileNameWithoutExtension(directoryModel.Name),
                LastModifiedDateTime = directoryModel.LastModifiedDateTime.ToString(CultureInfo.CurrentCulture),
            };

            return fileViewModel;
        }
    }
}