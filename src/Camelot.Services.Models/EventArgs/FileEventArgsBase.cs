namespace Camelot.Services.Models.EventArgs
{
    public class FileEventArgsBase : System.EventArgs
    {
        public string Node { get; }

        public FileEventArgsBase(string node)
        {
            Node = node;
        }
    }
}