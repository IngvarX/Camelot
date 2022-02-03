using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public class SelectionAddedEventArgs : EventArgs
{
    public string NodePath { get; }

    public SelectionAddedEventArgs(string nodePath)
    {
        NodePath = nodePath;
    }
}