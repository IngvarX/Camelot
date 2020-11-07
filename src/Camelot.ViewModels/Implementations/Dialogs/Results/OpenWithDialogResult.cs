using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Services;

namespace Camelot.ViewModels.Implementations.Dialogs.Results
{
    public class OpenWithDialogResult : DialogResultBase
    {
        public ApplicationModel Application { get; }

        public OpenWithDialogResult(ApplicationModel application)
        {
            Application = application;
        }
    }
}
