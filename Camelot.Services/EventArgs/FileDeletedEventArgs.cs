namespace Camelot.Services.EventArgs
{
    public class FileDeletedEventArgs : FileEventArgsBase
    {
        public FileDeletedEventArgs(string node)
            : base(node)
        {
        }
    }
}