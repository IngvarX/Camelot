using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationModel>> GetAssociatedApplicationsAsync(string fileExtension);

        Task<IEnumerable<ApplicationModel>> GetInstalledApplicationsAsync();
    }
}
