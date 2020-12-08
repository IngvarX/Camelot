using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;
using Camelot.Services.Linux.Interfaces;
using Camelot.Services.Linux.Specifications;

namespace Camelot.Services.Linux
{
    public class LinuxApplicationService : IApplicationService
    {
        private readonly IFileService _fileService;
        private readonly IIniReader _iniReader;
        private readonly IMimeTypesReader _mimeTypesReader;
        private readonly IPathService _pathService;

        private List<LinuxApplicationModel> _desktopEntries;

        public LinuxApplicationService(
            IFileService fileService,
            IIniReader iniReader,
            IMimeTypesReader mimeTypesReader,
            IPathService pathService)
        {
            _fileService = fileService;
            _iniReader = iniReader;
            _mimeTypesReader = mimeTypesReader;
            _pathService = pathService;
        }

        public async Task<IEnumerable<ApplicationModel>> GetAssociatedApplicationsAsync(string fileExtension)
        {
            var desktopEntries = await GetCachedDesktopEntriesAsync();

            var applications = desktopEntries
                .Where(m => m.Extensions.Contains(fileExtension))
                .ToList();

            var defaultApplicationIndex = applications.FindIndex(m => m.IsDefaultApplication);
            if (defaultApplicationIndex != -1)
            {
                (applications[0], applications[defaultApplicationIndex]) =
                    (applications[defaultApplicationIndex], applications[0]);
            }

            return applications;
        }

        public async Task<IEnumerable<ApplicationModel>> GetInstalledApplicationsAsync() =>
            await GetCachedDesktopEntriesAsync();

        private async Task<List<LinuxApplicationModel>> GetCachedDesktopEntriesAsync() =>
            _desktopEntries ??= await GetDesktopEntriesAsync();

        private static string ExtractArguments(string startCommand) => "{0}"; // TODO: fix %F => {0}

        private static string ExtractExecutePath(string startCommand) =>
            startCommand.Split().FirstOrDefault();

        private async Task<List<LinuxApplicationModel>> GetDesktopEntriesAsync()
        {
            var desktopEntryFiles = new List<LinuxApplicationModel>();

            var specification = GetSpecification();
            var desktopFilePaths = _fileService
                .GetFiles("/usr/share/applications/", specification)
                .Select(f => f.FullPath);

            var mimeTypesExtensions = await GetMimeTypesAsync();
            var defaultApplications = await GetDefaultApplicationsAsync(mimeTypesExtensions);

            foreach (var desktopFilePath in desktopFilePaths)
            {
                var desktopEntry = await GetDesktopEntryAsync(desktopFilePath);

                var desktopType = desktopEntry.GetValueOrDefault("Desktop Entry:Type");
                if (desktopType is null || !desktopType.Equals("Application", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var displayName = desktopEntry.GetValueOrDefault("Desktop Entry:Name");
                var startCommand = desktopEntry.GetValueOrDefault("Desktop Entry:Exec");

                if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(startCommand))
                {
                    continue;
                }

                var executePath = ExtractExecutePath(startCommand);
                if (string.IsNullOrWhiteSpace(executePath))
                {
                    continue;
                }

                var arguments = ExtractArguments(startCommand);
                var extensions = GetExtensions(desktopEntry, mimeTypesExtensions);

                var desktopFileName = _pathService.GetFileName(desktopFilePath);
                var isDefaultApplication = defaultApplications.ContainsKey(desktopFileName) &&
                                           defaultApplications[desktopFileName].IsSubsetOf(extensions);

                desktopEntryFiles.Add(new LinuxApplicationModel
                {
                    DisplayName = displayName,
                    ExecutePath = executePath,
                    Arguments = arguments,
                    Extensions = extensions,
                    IsDefaultApplication = isDefaultApplication
                });
            }

            return desktopEntryFiles;
        }

        private async Task<IReadOnlyDictionary<string, List<string>>> GetMimeTypesAsync()
        {
            var mimeTypesFile = _fileService.OpenRead("/etc/mime.types");

            return await _mimeTypesReader.ReadAsync(mimeTypesFile);
        }

        private async Task<IReadOnlyDictionary<string, HashSet<string>>> GetDefaultApplicationsAsync(
            IReadOnlyDictionary<string, List<string>> mimeTypesExtensions)
        {
            var defaultApplicationsByExtensions = new Dictionary<string, HashSet<string>>();
            var defaultsApplicationsListByMimeType = await GetDesktopEntryAsync("/usr/share/applications/defaults.list");

            foreach (var (key, value) in defaultsApplicationsListByMimeType)
            {
                var mimeType = key.Split(":").LastOrDefault();

                if (mimeType is null || !mimeTypesExtensions.TryGetValue(mimeType, out var extensions))
                {
                    continue;
                }

                if (defaultApplicationsByExtensions.ContainsKey(value))
                {
                    defaultApplicationsByExtensions[value].UnionWith(extensions);
                }
                else
                {
                    defaultApplicationsByExtensions.Add(value, new HashSet<string>(extensions));
                }
            }

            return defaultApplicationsByExtensions;
        }

        private async Task<IReadOnlyDictionary<string, string>> GetDesktopEntryAsync(string desktopFilePath)
        {
            await using var desktopFile = _fileService.OpenRead(desktopFilePath);

            return await _iniReader.ReadAsync(desktopFile);
        }

        private static HashSet<string> GetExtensions(
            IReadOnlyDictionary<string, string> desktopEntry,
            IReadOnlyDictionary<string, List<string>> mimeTypesExtensions)
        {
            var extensions = new HashSet<string>();

            var mimeTypes = desktopEntry.GetValueOrDefault("Desktop Entry:MimeType")
                ?.Split(';', StringSplitOptions.RemoveEmptyEntries);
            if (mimeTypes is null)
            {
                return extensions;
            }

            foreach (var mimeType in mimeTypes)
            {
                var extensionsByMimeType = mimeTypesExtensions.GetValueOrDefault(mimeType);
                extensionsByMimeType?.ForEach(e => extensions.Add(e));
            }

            return extensions;
        }

        private static ISpecification<FileModel> GetSpecification() => new DesktopFileSpecification();

        private class LinuxApplicationModel : ApplicationModel
        {
            public HashSet<string> Extensions { get; set; }

            public bool IsDefaultApplication { get; set; }
        }
    }
}
