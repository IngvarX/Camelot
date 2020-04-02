using Camelot.Services.Behaviors.Interfaces;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class FileViewModel : FileSystemNodeViewModelBase
    {
        public string Extension { get; set; }

        public string Size { get; set; }

        public FileViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesOperationsMediator filesOperationsMediator)
            : base(
                fileSystemNodeOpeningBehavior,
                operationsService,
                clipboardOperationsService,
                filesOperationsMediator)
        {
            
        }
    }
}