namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.EventArgs;

public class SelectionAddedEventArgs : System.EventArgs
{
    public string NodePath { get; }

    public SelectionAddedEventArgs(string nodePath)
    {
        NodePath = nodePath;
    }
}