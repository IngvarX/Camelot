using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public class SelectionRemovedEventArgs : EventArgs
{
    public string NodePath { get; }

    public SelectionRemovedEventArgs(string nodePath)
    {
        NodePath = nodePath;
    }
}