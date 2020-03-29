using Camelot.Services.Behaviors.Interfaces;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class FileViewModel : FileSystemNodeViewModelBase
    {
        public string Extension { get; set; }

        public string Size { get; set; }

        public FileViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior)
            : base(fileSystemNodeOpeningBehavior)
        {
            
        }
    }
}