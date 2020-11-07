using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.Results
{
    public class OpenWithDialogResult : DialogResultBase
    {
        public string FileExtension { get; }

        public ApplicationModel Application { get; }

        public bool IsDefaultApplication { get; }

        public OpenWithDialogResult(string fileExtension, ApplicationModel application, bool defaultApplication)
        {
            FileExtension = fileExtension;
            Application = application;
            IsDefaultApplication = defaultApplication;
        }
    }
}
