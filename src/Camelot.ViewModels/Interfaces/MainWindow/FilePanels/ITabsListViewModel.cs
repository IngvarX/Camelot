using System;
using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface ITabsListViewModel
    {
        ITabViewModel SelectedTab { get; }

        event EventHandler<EventArgs> SelectedTabChanged;

        ICommand SelectTabToTheLeftCommand { get; }

        ICommand SelectTabToTheRightCommand { get; }

        void CreateNewTab(string directory = null);

        void CloseActiveTab();

        void SaveState();
    }
}