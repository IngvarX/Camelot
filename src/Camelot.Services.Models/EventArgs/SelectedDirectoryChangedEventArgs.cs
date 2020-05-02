namespace Camelot.Services.Models.EventArgs
{
    public class SelectedDirectoryChangedEventArgs : System.EventArgs
    {
        public string NewDirectory { get; }

        public SelectedDirectoryChangedEventArgs(string newDirectory)
        {
            NewDirectory = newDirectory;
        }
    }
}