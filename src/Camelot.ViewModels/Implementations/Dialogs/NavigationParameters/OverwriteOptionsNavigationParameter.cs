using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters
{
    public class OverwriteOptionsNavigationParameter : NavigationParameterBase
    {
        public string SourceFilePath { get; }

        public string DestinationFilePath { get; }

        public OverwriteOptionsNavigationParameter(string sourceFilePath, string destinationFilePath)
        {
            SourceFilePath = sourceFilePath;
            DestinationFilePath = destinationFilePath;
        }
    }
}