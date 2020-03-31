using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class DirectoryViewModel : FileSystemNodeViewModelBase
    {
        public DirectoryViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IOperationsService operationsService) 
            : base(fileSystemNodeOpeningBehavior, operationsService)
        {
            
        }
    }
}