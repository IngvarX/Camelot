using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Windows.Enums;
using Camelot.Services.Windows.Interfaces;

namespace Camelot.Services.Windows
{
    public class WindowsApplicationService : IApplicationService
    {
        private const string FileExtensionsX32RegistryKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
        private const string FileExtensionsX64RegistryKeyName = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
        private const string AppPathX32RegistryKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
        private const string AppPathX64RegistryKeyName = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths";

        private readonly IEnvironmentService _environmentService;
        private readonly IRegexService _regexService;
        private readonly IApplicationInfoProvider _applicationInfoProvider;
        private readonly IRegistryService _registryService;

        public WindowsApplicationService(
            IEnvironmentService environmentService,
            IRegexService regexService,
            IApplicationInfoProvider applicationInfoProvider,
            IRegistryService registryService)
        {
            _environmentService = environmentService;
            _regexService = regexService;
            _applicationInfoProvider = applicationInfoProvider;
            _registryService = registryService;
        }

        public Task<IEnumerable<ApplicationModel>> GetAssociatedApplicationsAsync(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                return Task.FromResult(Enumerable.Empty<ApplicationModel>());
            }

            if (!fileExtension.StartsWith("."))
            {
                fileExtension = $".{fileExtension}";
            }

            var associatedApplications = new Dictionary<string, ApplicationModel>();

            TryAddApplications(fileExtension, associatedApplications);

            return Task.FromResult<IEnumerable<ApplicationModel>>(associatedApplications.Values);
        }

        public Task<IEnumerable<ApplicationModel>> GetInstalledApplicationsAsync()
        {
            var installedApplications = new Dictionary<string, ApplicationModel>();

            var applicationsFiles = GetApplicationsFiles(RootRegistryKey.ClassesRoot, "Applications")
                .Union(GetApplicationsFiles(RootRegistryKey.LocalMachine, AppPathX32RegistryKeyName))
                .Union(GetApplicationsFiles(RootRegistryKey.CurrentUser, AppPathX32RegistryKeyName))
                .ToImmutableHashSet();

            if (_environmentService.Is64BitProcess)
            {
                applicationsFiles = applicationsFiles
                    .Union(GetApplicationsFiles(RootRegistryKey.LocalMachine, AppPathX64RegistryKeyName))
                    .Union(GetApplicationsFiles(RootRegistryKey.CurrentUser, AppPathX64RegistryKeyName))
                    .ToImmutableHashSet();
            }

            foreach (var applicationFile in applicationsFiles)
            {
                TryAddApplication(installedApplications, applicationFile);
            }

            return Task.FromResult<IEnumerable<ApplicationModel>>(installedApplications.Values);
        }
        
        private void TryAddApplications(string s, IDictionary<string, ApplicationModel> associatedApplications)
        {
            foreach (var applicationFile in GetOpenWithList(s, RootRegistryKey.CurrentUser,
                FileExtensionsX32RegistryKeyName))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }

            foreach (var applicationFile in GetOpenWithProgids(s, RootRegistryKey.CurrentUser,
                FileExtensionsX32RegistryKeyName))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }

            if (_environmentService.Is64BitProcess)
            {
                foreach (var applicationFile in GetOpenWithList(s, RootRegistryKey.CurrentUser,
                    FileExtensionsX64RegistryKeyName))
                {
                    TryAddApplication(associatedApplications, applicationFile);
                }

                foreach (var applicationFile in GetOpenWithProgids(s, RootRegistryKey.CurrentUser,
                    FileExtensionsX64RegistryKeyName))
                {
                    TryAddApplication(associatedApplications, applicationFile);
                }
            }

            foreach (var applicationFile in GetOpenWithList(s, RootRegistryKey.ClassesRoot))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }

            foreach (var applicationFile in GetOpenWithProgids(s, RootRegistryKey.ClassesRoot))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }
        }
        
        private IEnumerable<string> GetApplicationsFiles(RootRegistryKey rootKey, string baseKeyName)
        {
            var applications = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            using var applicationsKeys = _registryService.GetRegistryKey(rootKey).OpenSubKey(baseKeyName);
            foreach (var appExecuteFile in applicationsKeys.GetSubKeyNames())
            {
                applications.Add(appExecuteFile);
            }

            return applications.ToImmutableHashSet();
        }
        
        private IEnumerable<string> GetOpenWithList(string fileExtension, RootRegistryKey rootKey,
            string baseKeyName = "")
        {
            var results = new List<string>();

            baseKeyName = @$"{baseKeyName?.TrimEnd('\\')}\{fileExtension}\OpenWithList";

            using var baseKey = _registryService.GetRegistryKey(rootKey).OpenSubKey(baseKeyName);
            if (!(baseKey?.GetValue("MRUList") is string mruList))
            {
                return results;
            }

            foreach (var mru in mruList)
            {
                if (baseKey.GetValue(mru.ToString()) is string name)
                {
                    results.Add(name);
                }
            }

            return results;
        }
        
        private IEnumerable<string> GetOpenWithProgids(string fileExtension, RootRegistryKey rootKey,
            string baseKeyName = "")
        {
            var results = new List<string>();

            baseKeyName = @$"{baseKeyName?.TrimEnd('\\')}\{fileExtension}\OpenWithProgids";

            using var baseKey = _registryService.GetRegistryKey(rootKey).OpenSubKey(baseKeyName);
            if (baseKey != null)
            {
                results.AddRange(baseKey.GetValueNames());
            }

            return results;
        }

        private void TryAddApplication(IDictionary<string, ApplicationModel> applications, string applicationFile)
        {
            var application = FindApplication(applicationFile);
            if (application != null)
            {
                applications.TryAdd(application.DisplayName, application);
            }
        }

        private ApplicationModel FindApplication(string applicationFile)
        {
            var info = _applicationInfoProvider.GetInfo(applicationFile);
            if (info == default)
            {
                return null;
            }
            
            return new ApplicationModel
            {
                DisplayName = info.Name,
                ExecutePath = info.ExecutePath,
                Arguments = ExtractArguments(info.StartCommand, info.ExecutePath)
            };
        }
        
        private string ExtractArguments(string path, string executePath)
        {
            path = path
                .Replace("\"", "")
                .Replace(executePath, "")
                .TrimStart();

            var argumentsCount = 0;
                
            return _regexService
                .GetMatches(path, "%.", RegexOptions.Compiled)
                .Aggregate(path, (current, match) => current.Replace(match.Value, $"{{{argumentsCount++}}}"));
        }
    }
}