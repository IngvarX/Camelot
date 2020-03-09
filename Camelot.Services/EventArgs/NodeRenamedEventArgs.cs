namespace Camelot.Services.EventArgs
{
    public class NodeRenamedEventArgs : NodeEventArgsBase
    {
        public string NewName { get; }

        public NodeRenamedEventArgs(string node, string newName)
            : base(node)
        {
            NewName = newName;
        }
    }
}