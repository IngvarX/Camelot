using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class DirectoryViewModel : FileSystemNodeViewModelBase, IDirectoryViewModel
    {
        private readonly IFilesOperationsMediator _filesOperationsMediator;

        public bool IsParentDirectory { get; set; }

        public ICommand OpenInNewTabCommand { get; }

        public ICommand OpenInNewTabOnOtherPanelCommand { get; }

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
                pathService,
                true)
        {
            _filesOperationsMediator = filesOperationsMediator;

            OpenInNewTabCommand = ReactiveCommand.Create(OpenInNewTab);
            OpenInNewTabOnOtherPanelCommand = ReactiveCommand.Create(OpenInNewTabOnOtherPanel);
        }

        private void OpenInNewTab() => OpenTab(_filesOperationsMediator.ActiveFilesPanelViewModel);

        private void OpenInNewTabOnOtherPanel() => OpenTab(_filesOperationsMediator.InactiveFilesPanelViewModel);

        private void OpenTab(IFilesPanelViewModel panel) => panel.TabsListViewModel.CreateNewTab(FullPath);
    }
}