using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Mac.Interfaces
{
    public interface IApplicationsAssociationsLoader
    {
        Task<IReadOnlyDictionary<string, ISet<ApplicationModel>>> LoadAssociatedApplicationsAsync(
            IEnumerable<ApplicationModel> installedApps);
    }
}