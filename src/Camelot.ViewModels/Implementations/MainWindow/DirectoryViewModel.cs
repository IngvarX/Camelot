using Camelot.Services.Behaviors.Interfaces;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class DirectoryViewModel : FileSystemNodeViewModelBase
    {
        public DirectoryViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior) 
            : base(fileSystemNodeOpeningBehavior)
        {
            
        }
    }
}