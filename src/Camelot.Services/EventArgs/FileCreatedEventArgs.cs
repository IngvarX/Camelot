namespace Camelot.Services.EventArgs
{
    public class FileCreatedEventArgs : FileEventArgsBase
    {
        public FileCreatedEventArgs(string node)
            : base(node)
        {
        }
    }
}