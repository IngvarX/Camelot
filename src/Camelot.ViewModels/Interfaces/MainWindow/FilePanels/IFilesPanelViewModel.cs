using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IFilesPanelViewModel
    {
        ITabsListViewModel TabsListViewModel { get; }

        string CurrentDirectory { get; set; }

        event EventHandler<EventArgs> ActivatedEvent;

        event EventHandler<EventArgs> CurrentDirectoryChanged;

        void Activate();

        void Deactivate();

        void OpenLastSelectedFile();
    }
}