using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters
{
    public class CreateDirectoryNavigationParameter : NavigationParameterBase
    {
        public string DirectoryPath { get; }

        public CreateDirectoryNavigationParameter(string directoryPath)
        {
            DirectoryPath = directoryPath;
        }
    }
}