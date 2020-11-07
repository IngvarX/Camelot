using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface IOpenWithApplicationService
    {
        ApplicationModel GetSelectedApplication(string fileExtension);

        void SaveSelectedApplication(string fileExtension, ApplicationModel selectedApplication);
    }
}
