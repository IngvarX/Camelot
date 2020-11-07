using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class DirectoryViewModel : FileSystemNodeViewModelBase, IDirectoryViewModel
    {
        public bool IsParentDirectory { get; set; }

        public DirectoryViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesOperationsMediator filesOperationsMediator,
            IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
            IDialogService dialogService,
            ITrashCanService trashCanService,
            IArchiveService archiveService,
            ISystemDialogService systemDialogService,
            IOpenWithApplicationService openWithApplicationService,
            IPathService pathService)
            : base(
                fileSystemNodeOpeningBehavior,
                operationsService,
                clipboardOperationsService,
                filesOperationsMediator,
                fileSystemNodePropertiesBehavior,
                dialogService,
                trashCanService,
                archiveService,
                systemDialogService,
                openWithApplicationService,
                pathService)
        {

        }
    }
}