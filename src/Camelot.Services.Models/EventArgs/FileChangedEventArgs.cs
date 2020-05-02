namespace Camelot.Services.Models.EventArgs
{
    public class FileChangedEventArgs : FileEventArgsBase
    {
        public FileChangedEventArgs(string node)
            : base(node)
        {
        }
    }
}