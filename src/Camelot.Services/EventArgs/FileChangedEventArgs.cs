namespace Camelot.Services.EventArgs
{
    public class FileChangedEventArgs : FileEventArgsBase
    {
        public FileChangedEventArgs(string node)
            : base(node)
        {
        }
    }
}