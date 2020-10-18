using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters
{
    public class CreateArchiveNavigationParameter : NavigationParameterBase
    {
        public string DefaultArchivePath { get; }

        public CreateArchiveNavigationParameter(string defaultArchivePath)
        {
            DefaultArchivePath = defaultArchivePath;
        }
    }
}