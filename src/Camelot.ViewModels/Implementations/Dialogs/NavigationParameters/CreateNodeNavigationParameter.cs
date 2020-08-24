using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters
{
    public class CreateNodeNavigationParameter : NavigationParameterBase
    {
        public string DirectoryPath { get; }

        public CreateNodeNavigationParameter(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }
    }
}