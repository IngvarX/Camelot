using System;

namespace Camelot.ViewModels.Interfaces.MainWindow
{
    public interface ITabViewModel
    {
        bool IsActive { get; set; }
        
        string CurrentDirectory { get; set; }
        
        event EventHandler<EventArgs> ActivationRequested;
        
        event EventHandler<EventArgs> NewTabRequested;
        
        event EventHandler<EventArgs> CloseRequested;

        event EventHandler<EventArgs> ClosingTabsToTheLeftRequested;
        
        event EventHandler<EventArgs> ClosingTabsToTheRightRequested;
        
        event EventHandler<EventArgs> ClosingAllTabsButThisRequested;
    }
}