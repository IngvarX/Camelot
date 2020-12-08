using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Mac.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacApplicationService : IApplicationService
    {
        private readonly IApplicationsListLoader _applicationsListLoader;
        private readonly IApplicationsAssociationsLoader _applicationsAssociationsLoader;

        private IReadOnlyDictionary<string, ISet<ApplicationModel>> _associatedApplications;
        private IImmutableSet<ApplicationModel> _installedApplications;

        public MacApplicationService(
            IApplicationsListLoader applicationsListLoader,
            IApplicationsAssociationsLoader applicationsAssociationsLoader)
        {
            _applicationsListLoader = applicationsListLoader;
            _applicationsAssociationsLoader = applicationsAssociationsLoader;
        }

        public async Task<IEnumerable<ApplicationModel>> GetAssociatedApplicationsAsync(string fileExtension)
        {
            _associatedApplications ??= await LoadAssociatedApplicationsAsync();

            return _associatedApplications.GetValueOrDefault(fileExtension, new HashSet<ApplicationModel>());
        }

        public Task<IEnumerable<ApplicationModel>> GetInstalledApplicationsAsync()
        {
            _installedApplications ??= _applicationsListLoader.GetInstalledApplications();

            return Task.FromResult((IEnumerable<ApplicationModel>) _installedApplications);
        }
        
        private async Task<IReadOnlyDictionary<string, ISet<ApplicationModel>>> LoadAssociatedApplicationsAsync()
        {
            var installedApps = await GetInstalledApplicationsAsync();
            
            return await _applicationsAssociationsLoader.LoadAssociatedApplicationsAsync(installedApps);
        }
    }
}
