using Camelot.Factories.Interfaces;
using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Models;
using Camelot.ViewModels.MainWindow;

namespace Camelot.Factories.Implementations
{
    public class FileViewModelFactory : IFileViewModelFactory
    {
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
            var fileViewModel = new FileViewModel(_fileOpeningBehavior,
                fileModel.FullPath, fileModel.LastModifiedDateTime);

            return fileViewModel;
        }

        public FileViewModel Create(DirectoryModel fileModel)
        {
            var fileViewModel = new FileViewModel(_directoryOpeningBehavior,
                fileModel.FullPath, fileModel.LastModifiedDateTime);

            return fileViewModel;
        }
    }
}