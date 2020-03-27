using System.Globalization;
using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.MainWindow;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class FileViewModelFactory : IFileViewModelFactory
    {
        private const string DirectoryFakeSize = "<DIR>";

        private readonly IFileOpeningBehavior _fileOpeningBehavior;
        private readonly IFileOpeningBehavior _directoryOpeningBehavior;
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IPathService _pathService;

        public FileViewModelFactory(
            IFileOpeningBehavior fileOpeningBehavior,
            IFileOpeningBehavior directoryOpeningBehavior,
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService)
        {
            _fileOpeningBehavior = fileOpeningBehavior;
            _directoryOpeningBehavior = directoryOpeningBehavior;
            _fileSizeFormatter = fileSizeFormatter;
            _pathService = pathService;
        }

        public FileViewModel Create(FileModel fileModel)
        {
            var fileViewModel = new FileViewModel(_fileOpeningBehavior)
            {
                FullPath = fileModel.FullPath,
                Size = _fileSizeFormatter.GetFormattedSize(fileModel.SizeBytes),
                LastModifiedDateTime = fileModel.LastWriteTime.ToString(CultureInfo.CurrentCulture),
                FileName = _pathService.GetFileNameWithoutExtension(fileModel.Name),
                Extension = _pathService.GetExtension(fileModel.Name)
            };

            return fileViewModel;
        }

        public FileViewModel Create(DirectoryModel directoryModel)
        {
            var fileViewModel = new FileViewModel(_directoryOpeningBehavior)
            {
                FullPath = directoryModel.FullPath,
                Size = DirectoryFakeSize,
                LastModifiedDateTime = directoryModel.LastModifiedDateTime.ToString(CultureInfo.CurrentCulture),
                FileName = directoryModel.Name,
                Extension = string.Empty
            };

            return fileViewModel;
        }
    }
}