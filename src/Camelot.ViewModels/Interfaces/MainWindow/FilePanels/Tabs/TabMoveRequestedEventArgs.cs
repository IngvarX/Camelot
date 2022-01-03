using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;

public class TabMoveRequestedEventArgs : EventArgs
{
    public ITabViewModel Target { get; }

    public TabMoveRequestedEventArgs(ITabViewModel target)
    {
        Target = target;
    }
}