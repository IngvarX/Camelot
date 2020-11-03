using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters
{
    public class OpenWithNavigationParameter : NavigationParameterBase
    {
        public string FileExtension { get; set; }

        public OpenWithNavigationParameter(string fileExtension)
        {
            FileExtension = fileExtension;
        }
    }
}
