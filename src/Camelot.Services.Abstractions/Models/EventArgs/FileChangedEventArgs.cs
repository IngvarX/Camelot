namespace Camelot.Services.Abstractions.Models.EventArgs;

public class FileChangedEventArgs : FileEventArgsBase
{
    public FileChangedEventArgs(string node)
        : base(node)
    {
    }
}