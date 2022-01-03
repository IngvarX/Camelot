using System;
using System.Windows.Input;
using Camelot.Services.Abstractions.Models.State;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;

public interface ITabViewModel
{
    bool IsActive { get; set; }

    bool IsGloballyActive { get; set; }

    string CurrentDirectory { get; set; }

    IFileSystemNodesSortingViewModel SortingViewModel { get; }

    event EventHandler<EventArgs> ActivationRequested;

    event EventHandler<EventArgs> NewTabRequested;

    event EventHandler<EventArgs> NewTabOnOppositePanelRequested;

    event EventHandler<EventArgs> CloseRequested;

    event EventHandler<EventArgs> ClosingTabsToTheLeftRequested;

    event EventHandler<EventArgs> ClosingTabsToTheRightRequested;

    event EventHandler<EventArgs> ClosingAllTabsButThisRequested;

    event EventHandler<TabMoveRequestedEventArgs> MoveRequested;

    ICommand ActivateCommand { get; }

    ICommand CloseTabCommand { get; }

    ICommand RequestMoveCommand { get; }

    ICommand GoToPreviousDirectoryCommand { get; }

    ICommand GoToNextDirectoryCommand { get; }

    TabStateModel GetState();
}