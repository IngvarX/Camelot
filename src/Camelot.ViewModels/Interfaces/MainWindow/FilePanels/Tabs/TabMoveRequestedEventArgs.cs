namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;

public class TabMoveRequestedEventArgs : System.EventArgs
{
    public ITabViewModel Target { get; }

    public TabMoveRequestedEventArgs(ITabViewModel target)
    {
        Target = target;
    }
}