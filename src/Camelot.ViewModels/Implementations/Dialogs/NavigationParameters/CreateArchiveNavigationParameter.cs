using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters
{
    public class CreateArchiveNavigationParameter : NavigationParameterBase
    {
        public string DefaultArchivePath { get; }

        public bool IsPackingSingleFile { get; }

        public CreateArchiveNavigationParameter(string defaultArchivePath, bool isPackingSingleFile)
        {
            DefaultArchivePath = defaultArchivePath;
            IsPackingSingleFile = isPackingSingleFile;
        }
    }
}