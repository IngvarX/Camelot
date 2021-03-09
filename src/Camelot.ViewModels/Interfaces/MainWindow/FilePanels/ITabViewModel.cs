using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface ITabViewModel
    {
        bool IsActive { get; set; }

        bool IsGloballyActive { get; set; }

        string CurrentDirectory { get; set; }

        IFileSystemNodesSortingViewModel SortingViewModel { get; }

        event EventHandler<EventArgs> ActivationRequested;

        event EventHandler<EventArgs> NewTabRequested;

        event EventHandler<EventArgs> NewTabOnOtherPanelRequested;

        event EventHandler<EventArgs> CloseRequested;

        event EventHandler<EventArgs> ClosingTabsToTheLeftRequested;

        event EventHandler<EventArgs> ClosingTabsToTheRightRequested;

        event EventHandler<EventArgs> ClosingAllTabsButThisRequested;
    }
}