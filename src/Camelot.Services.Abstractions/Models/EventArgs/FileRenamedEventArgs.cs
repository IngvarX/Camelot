namespace Camelot.Services.Abstractions.Models.EventArgs;

public class FileRenamedEventArgs : FileEventArgsBase
{
    public string NewName { get; }

    public FileRenamedEventArgs(string node, string newName)
        : base(node)
    {
        NewName = newName;
    }
}