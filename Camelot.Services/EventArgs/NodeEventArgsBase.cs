namespace Camelot.Services.EventArgs
{
    public class NodeEventArgsBase : System.EventArgs
    {
        public string Node { get; }

        public NodeEventArgsBase(string node)
        {
            Node = node;
        }
    }
}