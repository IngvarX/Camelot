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

    event EventHandler<System.EventArgs> ActivationRequested;

    event EventHandler<System.EventArgs> NewTabRequested;

    event EventHandler<System.EventArgs> NewTabOnOppositePanelRequested;

    event EventHandler<System.EventArgs> CloseRequested;

    event EventHandler<System.EventArgs> ClosingTabsToTheLeftRequested;

    event EventHandler<System.EventArgs> ClosingTabsToTheRightRequested;

    event EventHandler<System.EventArgs> ClosingAllTabsButThisRequested;

    event EventHandler<TabMoveRequestedEventArgs> MoveRequested;

    ICommand ActivateCommand { get; }

    ICommand CloseTabCommand { get; }

    ICommand RequestMoveCommand { get; }

    ICommand GoToPreviousDirectoryCommand { get; }

    ICommand GoToNextDirectoryCommand { get; }

    TabStateModel GetState();
}