using System.IO;
using Camelot.Factories.Interfaces;
using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Models;
using Camelot.ViewModels.MainWindow;

namespace Camelot.Factories.Implementations
{
    public class FileViewModelFactory : IFileViewModelFactory
    {
        private const string DirectoryFakeSize = "<DIR>";

        private readonly IFileOpeningBehavior _fileOpeningBehavior;
        private readonly IFileOpeningBehavior _directoryOpeningBehavior;

        public FileViewModelFactory(
            IFileOpeningBehavior fileOpeningBehavior,
            IFileOpeningBehavior directoryOpeningBehavior)
        {
            _fileOpeningBehavior = fileOpeningBehavior;
            _directoryOpeningBehavior = directoryOpeningBehavior;
        }

        public FileViewModel Create(FileModel fileModel)
        {
            var file = new FileInfo(fileModel.FullPath);
            var fileViewModel = new FileViewModel(_fileOpeningBehavior)
            {
                FullPath = file.FullName,
                Size = file.Length.ToString(),
                LastModifiedDateTime = file.LastWriteTime.ToString(),
                FileName = file.Name.StartsWith(".") ? file.Name : Path.GetFileNameWithoutExtension(file.Name),
                Extension = file.Name.StartsWith(".") ? string.Empty :
                    file.Extension.Length > 1 ? file.Extension.Substring(1) : file.Extension
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