using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Mac.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacApplicationsListLoader : IApplicationsListLoader
    {
        private readonly IDirectoryService _directoryService;

        public MacApplicationsListLoader(
            IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }

        public IImmutableSet<ApplicationModel> GetInstalledApplications()
        {
            var userApps = GetUserApps();
            var systemApps = GetSystemApps();

            return userApps.Concat(systemApps).ToImmutableHashSet();
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