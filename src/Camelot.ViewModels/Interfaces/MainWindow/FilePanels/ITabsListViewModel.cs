using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface ITabsListViewModel
    {
        ITabViewModel SelectedTab { get; }

        event EventHandler<EventArgs> SelectedTabChanged;

        void CreateNewTab();

        void CloseActiveTab();

        void SaveState();
    }
}