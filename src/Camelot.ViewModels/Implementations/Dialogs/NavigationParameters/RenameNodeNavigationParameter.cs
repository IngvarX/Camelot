using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters
{
    public class RenameNodeNavigationParameter : NavigationParameterBase
    {
        public string NodePath { get; }

        public RenameNodeNavigationParameter(string nodePath)
        {
            NodePath = nodePath;
        }
    }
}