using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Microsoft.Win32;

namespace Camelot.Services.Windows
{
    public class WindowsApplicationService : IApplicationService
    {
        private const string RegkeyFileExtensionsX32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts";

        private const string RegkeyFileExtensionsX64 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\FileExts";

        public Task<IEnumerable<ApplicationModel>> GetAssociatedApplications(string fileExtension)
        {
            if (!Path.HasExtension(fileExtension))
            {
                throw new ArgumentException($"{nameof(fileExtension)} should be file extension.");
            }

            var associatedApplications = new Dictionary<string, ApplicationModel>();

            foreach (var exeName in GetOpenWithList(fileExtension, Registry.CurrentUser, RegkeyFileExtensionsX32))
            {
                TryAddApplication(exeName);
            }

            foreach (var progid in GetOpenWithProgids(fileExtension, Registry.CurrentUser, RegkeyFileExtensionsX32))
            {
                TryAddApplication(progid);
            }

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64 ||
                RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                foreach (var exeName in GetOpenWithList(fileExtension, Registry.CurrentUser, RegkeyFileExtensionsX64))
                {
                    TryAddApplication(exeName);
                }

                foreach (var progid in GetOpenWithProgids(fileExtension, Registry.CurrentUser, RegkeyFileExtensionsX64))
                {
                    TryAddApplication(progid);
                }
            }

            foreach (var exeName in GetOpenWithList(fileExtension, Registry.ClassesRoot))
            {
                TryAddApplication(exeName);
            }

            foreach (var progid in GetOpenWithProgids(fileExtension, Registry.ClassesRoot))
            {
                TryAddApplication(progid);
            }

            return Task.FromResult<IEnumerable<ApplicationModel>>(associatedApplications.Values);

            void TryAddApplication(string applicationName)
            {
                var application = FindApplication(applicationName);
                if (application != null)
                {
                    associatedApplications.TryAdd(applicationName, application);
                }
            }

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

            var startCommandsCache = GetCommandsCache(Registry.ClassesRoot)
                .Union(GetCommandsCache(Registry.LocalMachine, @"SOFTWARE\Classes"))
                .Union(GetCommandsCache(Registry.CurrentUser, @"SOFTWARE\Classes"))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64 ||
                RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                startCommandsCache = startCommandsCache
                    .Union(GetCommandsCache(Registry.LocalMachine, @"SOFTWARE\Wow6432Node\Classes"))
                    .Union(GetCommandsCache(Registry.CurrentUser, @"SOFTWARE\Wow6432Node\Classes"))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            foreach (var command in startCommandsCache)
            {
                TryAddApplication(command.Key);
            }

            return Task.FromResult<IEnumerable<ApplicationModel>>(installedApplications.Values);

            void TryAddApplication(string applicationName)
            {
                var application = FindApplication(applicationName);
                if (application != null)
                {
                    installedApplications.TryAdd(applicationName, application);
                }
            }

            static Dictionary<string, string> GetCommandsCache(RegistryKey rootKey, string baseKeyName = "")
            {
                var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

                baseKeyName = $@"{baseKeyName?.TrimEnd('\\')}\Applications";

                using var applicationsKeys = rootKey.OpenSubKey(baseKeyName);
                if (applicationsKeys == null)
                {
                    return result;
                }

                foreach (var appExecuteFile in applicationsKeys.GetSubKeyNames())
                {
                    using var commandKey = rootKey.OpenSubKey($@"{baseKeyName}\{appExecuteFile}\Shell\Open\Command");

                    var command = commandKey?.GetValue("") as string;
                    if (!string.IsNullOrWhiteSpace(command))
                    {
                        result.TryAdd(appExecuteFile, command);
                    }
                }

                return result;
            }
        }

        private static ApplicationModel FindApplication(string applicationName)
        {
            var assocFlag = Win32Api.AssocF.None;
            if (applicationName.Contains(".exe"))
            {
                assocFlag = Win32Api.AssocF.Open_ByExeName;
            }

            var startCommand = Win32Api.AssocQueryString(assocFlag, Win32Api.AssocStr.Command, applicationName);
            var displayName = Win32Api.AssocQueryString(assocFlag, Win32Api.AssocStr.FriendlyAppName, applicationName);

            if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(startCommand))
            {
                return null;
            }

            var executePath = Win32Api.AssocQueryString(assocFlag, Win32Api.AssocStr.Executable, applicationName);

            return new ApplicationModel
            {
                DisplayName = displayName,
                ExecutePath = executePath,
                StartCommand = startCommand
            };
        }

        private static class Win32Api
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