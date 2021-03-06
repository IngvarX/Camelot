using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface ITabsListViewModel
    {
        ITabViewModel SelectedTab { get; }

        IReadOnlyList<ITabViewModel> Tabs { get; }

        event EventHandler<EventArgs> SelectedTabChanged;

        ICommand SelectTabToTheLeftCommand { get; }

        ICommand SelectTabToTheRightCommand { get; }

        void CreateNewTab(string directory = null, bool switchTo = false);

        void CloseActiveTab();

        void SelectTab(int index);

        void SaveState();
    }
}