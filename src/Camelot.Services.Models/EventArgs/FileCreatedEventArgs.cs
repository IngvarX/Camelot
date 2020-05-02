namespace Camelot.Services.Models.EventArgs
{
    public class FileCreatedEventArgs : FileEventArgsBase
    {
        public FileCreatedEventArgs(string node)
            : base(node)
        {
        }
    }
}