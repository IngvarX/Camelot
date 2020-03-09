namespace Camelot.Services.EventArgs
{
    public class NodeDeletedEventArgs : NodeEventArgsBase
    {
        public NodeDeletedEventArgs(string node)
            : base(node)
        {
        }
    }
}