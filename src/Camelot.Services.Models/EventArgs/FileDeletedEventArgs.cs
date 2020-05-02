namespace Camelot.Services.Models.EventArgs
{
    public class FileDeletedEventArgs : FileEventArgsBase
    {
        public FileDeletedEventArgs(string node)
            : base(node)
        {
        }
    }
}