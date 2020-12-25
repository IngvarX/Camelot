using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.Results
{
    public class RenameNodeDialogResult : DialogResultBase
    {
        public string NodeName { get; }

        public RenameNodeDialogResult(string nodeName)
        {
            NodeName = nodeName;
        }
    }
}