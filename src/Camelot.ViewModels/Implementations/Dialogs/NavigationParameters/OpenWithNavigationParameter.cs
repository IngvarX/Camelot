using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.NavigationParameters
{
    public class OpenWithNavigationParameter : NavigationParameterBase
    {
        public string FileExtension { get; }

        public ApplicationModel Application { get; }

        public OpenWithNavigationParameter(string fileExtension, ApplicationModel application)
        {
            FileExtension = fileExtension;
            Application = application;
        }
    }
}
