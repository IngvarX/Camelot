using System;

namespace Camelot.ViewModels.Interfaces.MainWindow
{
    public interface IFilesPanelViewModel
    {
        string CurrentDirectory { get; set; }
        
        event EventHandler<EventArgs> ActivatedEvent;

        void Deactivate();
    }
}