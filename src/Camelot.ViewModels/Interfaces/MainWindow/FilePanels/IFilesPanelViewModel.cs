using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IFilesPanelViewModel
    {
        string CurrentDirectory { get; set; }
        
        event EventHandler<EventArgs> ActivatedEvent;

        void Activate();
        
        void Deactivate();

        void CreateNewTab();

        void CloseActiveTab();
    }
}