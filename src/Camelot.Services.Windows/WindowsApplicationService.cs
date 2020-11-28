using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Windows.WinApi;
using Microsoft.Win32;

namespace Camelot.Services.Windows
{
    public class WindowsApplicationService : IApplicationService
    {
        private const string FileExtensionsX32RegistryKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
        private const string FileExtensionsX64RegistryKeyName = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
        private const string AppPathX32RegistryKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
        private const string AppPathX64RegistryKeyName = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths";

        private readonly IEnvironmentService _environmentService;

        public WindowsApplicationService(
            IEnvironmentService environmentService)
        {
            _environmentService = environmentService;
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

            foreach (var applicationFile in GetOpenWithList(fileExtension, Registry.CurrentUser,
                FileExtensionsX32RegistryKeyName))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }

            foreach (var applicationFile in GetOpenWithProgids(fileExtension, Registry.CurrentUser,
                FileExtensionsX32RegistryKeyName))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }

            if (_environmentService.Is64BitProcess)
            {
                foreach (var applicationFile in GetOpenWithList(fileExtension, Registry.CurrentUser,
                    FileExtensionsX64RegistryKeyName))
                {
                    TryAddApplication(associatedApplications, applicationFile);
                }

                foreach (var applicationFile in GetOpenWithProgids(fileExtension, Registry.CurrentUser,
                    FileExtensionsX64RegistryKeyName))
                {
                    TryAddApplication(associatedApplications, applicationFile);
                }
            }

            foreach (var applicationFile in GetOpenWithList(fileExtension, Registry.ClassesRoot))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }

            foreach (var applicationFile in GetOpenWithProgids(fileExtension, Registry.ClassesRoot))
            {
                TryAddApplication(associatedApplications, applicationFile);
            }

            return Task.FromResult<IEnumerable<ApplicationModel>>(associatedApplications.Values);

            static IEnumerable<string> GetOpenWithList(string fileExtension, RegistryKey rootKey,
                string baseKeyName = "")
            {
                var results = new List<string>();

                baseKeyName = @$"{baseKeyName?.TrimEnd('\\')}\{fileExtension}\OpenWithList";

                using var baseKey = rootKey.OpenSubKey(baseKeyName);
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

            static IEnumerable<string> GetOpenWithProgids(string fileExtension, RegistryKey rootKey,
                string baseKeyName = "")
            {
                var results = new List<string>();

                baseKeyName = @$"{baseKeyName?.TrimEnd('\\')}\{fileExtension}\OpenWithProgids";

                using var baseKey = rootKey.OpenSubKey(baseKeyName);
                if (baseKey != null)
                {
                    results.AddRange(baseKey.GetValueNames());
                }

                return results;
            }
        }

        public Task<IEnumerable<ApplicationModel>> GetInstalledApplicationsAsync()
        {
            var installedApplications = new Dictionary<string, ApplicationModel>();

            var applicationsFiles = GetApplicationsFiles(Registry.ClassesRoot, "Applications")
                .Union(GetApplicationsFiles(Registry.LocalMachine, AppPathX32RegistryKeyName))
                .Union(GetApplicationsFiles(Registry.CurrentUser, AppPathX32RegistryKeyName))
                .ToImmutableHashSet();

            if (_environmentService.Is64BitProcess)
            {
                applicationsFiles = applicationsFiles
                    .Union(GetApplicationsFiles(Registry.LocalMachine, AppPathX64RegistryKeyName))
                    .Union(GetApplicationsFiles(Registry.CurrentUser, AppPathX64RegistryKeyName))
                    .ToImmutableHashSet();
            }

            foreach (var applicationFile in applicationsFiles)
            {
                TryAddApplication(installedApplications, applicationFile);
            }

            return Task.FromResult<IEnumerable<ApplicationModel>>(installedApplications.Values);

            static ImmutableHashSet<string> GetApplicationsFiles(RegistryKey rootKey, string baseKeyName)
            {
                var applications = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

                using var applicationsKeys = rootKey.OpenSubKey(baseKeyName);
                if (applicationsKeys != null)
                {
                    foreach (var appExecuteFile in applicationsKeys.GetSubKeyNames())
                    {
                        applications.Add(appExecuteFile);
                    }
                }

                return applications.ToImmutableHashSet();
            }
        }

        private static void TryAddApplication(IDictionary<string, ApplicationModel> applications, string applicationFile)
        {
            var application = FindApplication(applicationFile);
            if (application != null)
            {
                applications.TryAdd(application.DisplayName, application);
            }
        }

        private static ApplicationModel FindApplication(string applicationFile)
        {
            var assocFlag = Win32.AssocF.None;
            if (applicationFile.Contains(".exe"))
            {
                assocFlag = Win32.AssocF.OpenByExeName;
            }

            var displayName = Win32.AssocQueryString(assocFlag, Win32.AssocStr.FriendlyAppName, applicationFile);
            var startCommand = Win32.AssocQueryString(assocFlag, Win32.AssocStr.Command, applicationFile);

            if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(startCommand))
            {
                return null;
            }

            var executePath = Win32.AssocQueryString(assocFlag, Win32.AssocStr.Executable, applicationFile);

            return new ApplicationModel
            {
                DisplayName = displayName,
                ExecutePath = executePath,
                Arguments = ExtractArguments(startCommand)
            };

            string ExtractArguments(string path)
            {
                path = path
                    .Replace("\"", "")
                    .Replace(executePath, "")
                    .TrimStart();

                var argumentsCount = 0;
                var matches = Regex.Matches(path, "%.", RegexOptions.Compiled);
                foreach (Match match in matches)
                {
                    path = path.Replace(match.Value, $"{{{argumentsCount++}}}");
                }

                return path;
            }
        }
    }
}