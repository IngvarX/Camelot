using System.IO;
using Camelot.Factories.Interfaces;
using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;
using Camelot.Services.Models;
using Camelot.ViewModels.MainWindow;

namespace Camelot.Factories.Implementations
{
    public class FileViewModelFactory : IFileViewModelFactory
    {
        private const string DirectoryFakeSize = "<DIR>";

        private readonly IFileOpeningBehavior _fileOpeningBehavior;
        private readonly IFileOpeningBehavior _directoryOpeningBehavior;
        private readonly IFileSizeFormatter _fileSizeFormatter;

        public FileViewModelFactory(
            IFileOpeningBehavior fileOpeningBehavior,
            IFileOpeningBehavior directoryOpeningBehavior,
            IFileSizeFormatter fileSizeFormatter)
        {
            _fileOpeningBehavior = fileOpeningBehavior;
            _directoryOpeningBehavior = directoryOpeningBehavior;
            _fileSizeFormatter = fileSizeFormatter;
        }

        public FileViewModel Create(FileModel fileModel)
        {
            var fileViewModel = new FileViewModel(_fileOpeningBehavior)
            {
                FullPath = fileModel.FullPath,
                Size = _fileSizeFormatter.GetFormattedSize(fileModel.SizeBytes),
                LastModifiedDateTime = fileModel.LastWriteTime.ToString(),
                FileName = fileModel.Name.StartsWith(".") ? fileModel.Name : Path.GetFileNameWithoutExtension(fileModel.Name),
                Extension = fileModel.Name.StartsWith(".") ? string.Empty :
                    fileModel.Extension.Length > 1 ? fileModel.Extension.Substring(1) : fileModel.Extension
            };

            return fileViewModel;
        }

        public FileViewModel Create(DirectoryModel fileModel)
        {
            var fileViewModel = new FileViewModel(_directoryOpeningBehavior)
            {
                FullPath = fileModel.FullPath,
                Size = DirectoryFakeSize,
                LastModifiedDateTime = fileModel.LastModifiedDateTime.ToString(),
                FileName = Path.GetFileName(fileModel.FullPath),
                Extension = string.Empty
            };

            return fileViewModel;
        }
    }
}