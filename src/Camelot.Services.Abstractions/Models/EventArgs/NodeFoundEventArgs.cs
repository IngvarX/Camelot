namespace Camelot.Services.Abstractions.Models.EventArgs;

public class NodeFoundEventArgs : System.EventArgs
{
    public string NodePath { get; }

    public NodeFoundEventArgs(string nodePath)
    {
        NodePath = nodePath;
    }
}