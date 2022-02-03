namespace Camelot.Services.Abstractions.Models.EventArgs;

public class FileDeletedEventArgs : FileEventArgsBase
{
    public FileDeletedEventArgs(string node)
        : base(node)
    {
    }
}