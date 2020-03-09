namespace Camelot.Services.EventArgs
{
    public class NodeCreatedEventArgs : NodeEventArgsBase
    {
        public NodeCreatedEventArgs(string node)
            : base(node)
        {
        }
    }
}