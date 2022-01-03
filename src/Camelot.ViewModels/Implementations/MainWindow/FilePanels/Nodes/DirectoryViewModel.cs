using System.Windows.Input;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;

public class DirectoryViewModel : FileSystemNodeViewModelBase, IDirectoryViewModel
{
    private readonly IFilesOperationsMediator _filesOperationsMediator;

    public bool IsParentDirectory { get; set; }

    public ICommand OpenInNewTabCommand { get; }

    public ICommand OpenInNewTabOnOppositePanelCommand { get; }

    public DirectoryViewModel(
        IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
        IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
        IFileSystemNodeFacade fileSystemNodeFacade,
        bool shouldShowOpenSubmenu,
        IFilesOperationsMediator filesOperationsMediator)
        : base(
            fileSystemNodeOpeningBehavior,
            fileSystemNodePropertiesBehavior,
            fileSystemNodeFacade,
            shouldShowOpenSubmenu)
    {
        _filesOperationsMediator = filesOperationsMediator;

        OpenInNewTabCommand = ReactiveCommand.Create(OpenInNewTab);
        OpenInNewTabOnOppositePanelCommand = ReactiveCommand.Create(OpenInNewTabOnOppositePanel);
    }

    private void OpenInNewTab() => OpenTab(_filesOperationsMediator.ActiveFilesPanelViewModel);

    private void OpenInNewTabOnOppositePanel() => OpenTab(_filesOperationsMediator.InactiveFilesPanelViewModel);

    private void OpenTab(IFilesPanelViewModel panel) => panel.TabsListViewModel.CreateNewTab(FullPath);
}