using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Mac
{
    public class MacApplicationService : IApplicationService
    {
        private readonly IDirectoryService _directoryService;

        public MacApplicationService(
            IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        public Task<IEnumerable<ApplicationModel>> GetAssociatedApplicationsAsync(string fileExtension)
        {
            return Task.FromResult(Enumerable.Empty<ApplicationModel>());
        }

        public Task<IEnumerable<ApplicationModel>> GetInstalledApplicationsAsync()
        {
            var userApps = GetUserApps();
            var systemApps = GetSystemApps();

            return Task.FromResult(userApps.Concat(systemApps));
        }

        private IEnumerable<ApplicationModel> GetUserApps() => GetApps("/Applications/");

        private IEnumerable<ApplicationModel> GetSystemApps() => GetApps("/System/Applications/");

        private IEnumerable<ApplicationModel> GetApps(string directory) =>
            _directoryService
                .GetChildDirectories(directory)
                .Select(d => new ApplicationModel
                {
                    DisplayName = ExtractName(d.Name),
                    ExecutePath = d.FullPath,
                    Arguments = "{0}"
                });

        private static string ExtractName(string directory) =>
            directory.Replace(".app", string.Empty);
    }
}
