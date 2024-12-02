namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.EventArgs;

public class SelectionRemovedEventArgs : System.EventArgs
{
    public string NodePath { get; }

    public SelectionRemovedEventArgs(string nodePath)
    {
        NodePath = nodePath;
    }
}