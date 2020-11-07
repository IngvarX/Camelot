using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Microsoft.Win32;

namespace Camelot.Services.Windows
{
    public class WindowsApplicationService : IApplicationService
    {
        private const string FileExtensionsX32RegistryKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts";

        private const string FileExtensionsX64RegistryKeyName = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\FileExts";

        private const string AppPathX32RegistryKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";

        private const string AppPathX64RegistryKeyName = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths";

        public Task<IEnumerable<ApplicationModel>> GetAssociatedApplications(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
            {
                return Task.FromResult(Enumerable.Empty<ApplicationModel>());
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

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64 ||
                RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
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

        public Task<IEnumerable<ApplicationModel>> GetInstalledApplications()
        {
            var installedApplications = new Dictionary<string, ApplicationModel>();

            var applicationsFiles = GetApplicationsFiles(Registry.ClassesRoot, "Applications")
                .Union(GetApplicationsFiles(Registry.LocalMachine, AppPathX32RegistryKeyName))
                .Union(GetApplicationsFiles(Registry.CurrentUser, AppPathX32RegistryKeyName))
                .ToImmutableHashSet();

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64 ||
                RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
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
                assocFlag = Win32.AssocF.Open_ByExeName;
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

                path = Regex.Replace(path, "%.", "", RegexOptions.Compiled);
                return path;
            }
        }

        private static class Win32
        {
            public static string AssocQueryString(AssocF assocF, AssocStr association, string assocString)
            {
                var length = 0u;
                var queryResult = AssocQueryString(assocF, association, assocString, null, null, ref length);
                if (queryResult != 1)
                {
                    return null;
                }

                var builder = new StringBuilder((int) length);
                queryResult = AssocQueryString(assocF, association, assocString, null, builder, ref length);
                return queryResult != 0 ? null : builder.ToString();
            }

            [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
            private static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string pszExtra,
                [Out] StringBuilder pszOut, ref uint pcchOut);

            public enum AssocStr
            {
                Command = 1,
                Executable,
                FriendlyDocName,
                FriendlyAppName,
                NoOpen,
                ShellNewValue,
                DDECommand,
                DDEIfExec,
                DDEApplication,
                DDETopic,
                InfoTip,
                QuickTip,
                TileInfo,
                ContentType,
                DefaultIcon,
                ShellExtension,
                DropTarget,
                DelegateExecute,
                Supported_Uri_Protocols,
                ProgID,
                AppID,
                AppPublisher,
                AppIconReference,
                Max
            }

            [Flags]
            public enum AssocF
            {
                None = 0,
                Init_NoRemapCLSID = 0x1,
                Init_ByExeName = 0x2,
                Open_ByExeName = 0x2,
                Init_DefaultToStar = 0x4,
                Init_DefaultToFolder = 0x8,
                NoUserSettings = 0x10,
                NoTruncate = 0x20,
                Verify = 0x40,
                RemapRunDll = 0x80,
                NoFixUps = 0x100,
                IgnoreBaseClass = 0x200,
                Init_IgnoreUnknown = 0x400,
                Init_Fixed_ProgId = 0x800,
                Is_Protocol = 0x1000,
                Init_For_File = 0x2000
            }
        }
    }
}