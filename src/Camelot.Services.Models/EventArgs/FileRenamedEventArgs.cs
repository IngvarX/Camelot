namespace Camelot.Services.Models.EventArgs
{
    public class FileRenamedEventArgs : FileEventArgsBase
    {
        public string NewName { get; }

        public FileRenamedEventArgs(string node, string newName)
            : base(node)
        {
            NewName = newName;
        }
    }
}