namespace Camelot.Services.Abstractions.Models.EventArgs;

public class SelectedDirectoryChangedEventArgs : System.EventArgs
{
    public string NewDirectory { get; }

    public SelectedDirectoryChangedEventArgs(string newDirectory)
    {
        NewDirectory = newDirectory;
    }
}