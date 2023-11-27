namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.EventArgs;

public class QuickSearchFilterChangedEventArgs : System.EventArgs
{
    public SelectionChangeDirection Direction { get; }

    public QuickSearchFilterChangedEventArgs(SelectionChangeDirection direction)
    {
        Direction = direction;
    }
}