using Camelot.Services.Behaviors.Interfaces;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class FileViewModel : FileSystemNodeViewModelBase
    {
        public string FileName { get; set; }

        public string Extension { get; set; }

        public string Size { get; set; }

        public FileViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior)
            : base(fileSystemNodeOpeningBehavior)
        {
            
        }
    }
}