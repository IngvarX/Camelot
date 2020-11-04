using System;
using System.Collections.Generic;
using System.IO;
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
        private const string RegkeyInstalledApplicationsX32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string RegkeyInstalledApplicationsX64 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string RegkeyFileExtsX32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts";

        private const string RegkeyFileExtsX64 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Explorer\FileExts";

        public Task<IEnumerable<ApplicationModel>> GetAssociatedApplications(string fileExtension)
        {
            if (!Path.HasExtension(fileExtension))
            {
                throw new ArgumentException($"{nameof(fileExtension)} should be file extension.");
            }

            var associatedApplications = new Dictionary<string, ApplicationModel>();

            foreach (var exeName in GetOpenWithList(fileExtension, Registry.CurrentUser, RegkeyFileExtsX32))
            {
                TryAddApplication(exeName);
            }

            foreach (var progid in GetOpenWithProgids(fileExtension, Registry.CurrentUser, RegkeyFileExtsX32))
            {
                TryAddApplication(progid);
            }

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64 ||
                RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                foreach (var exeName in GetOpenWithList(fileExtension, Registry.CurrentUser, RegkeyFileExtsX64))
                {
                    TryAddApplication(exeName);
                }

                foreach (var progid in GetOpenWithProgids(fileExtension, Registry.CurrentUser, RegkeyFileExtsX64))
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
                var assocFlag = Win32Api.AssocF.None;
                if (applicationName.Contains(".exe"))
                {
                    assocFlag = Win32Api.AssocF.Open_ByExeName;
                }

                var startCommand = Win32Api.AssocQueryString(assocFlag, Win32Api.AssocStr.Command, applicationName);
                var displayName = Win32Api.AssocQueryString(assocFlag, Win32Api.AssocStr.FriendlyAppName, applicationName);

                if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(startCommand))
                {
                    return;
                }

                var displayIcon = Win32Api.AssocQueryString(assocFlag, Win32Api.AssocStr.DefaultIcon, applicationName);
                var executePath = Win32Api.AssocQueryString(assocFlag, Win32Api.AssocStr.Executable, applicationName);

                associatedApplications.TryAdd(displayName, new ApplicationModel
                {
                    FileExtension = fileExtension,
                    DisplayName = displayName,
                    DisplayIcon = displayIcon,
                    ExecutePath = executePath,
                    StartCommand = startCommand
                });
            }
        }

        public Task<IEnumerable<ApplicationModel>> GetInstalledApplications()
        {
            var installedApplications = new Dictionary<string, ApplicationModel>();

            AddApplications(Registry.CurrentUser, RegkeyInstalledApplicationsX32);
            AddApplications(Registry.LocalMachine, RegkeyInstalledApplicationsX32);

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64 ||
                RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                AddApplications(Registry.CurrentUser, RegkeyInstalledApplicationsX64);
                AddApplications(Registry.LocalMachine, RegkeyInstalledApplicationsX64);
            }

            return Task.FromResult<IEnumerable<ApplicationModel>>(installedApplications.Values);

            void AddApplications(RegistryKey rootKey, string baseKeyName)
            {
                using var baseApplicationsKey = rootKey.OpenSubKey(baseKeyName);
                if (baseApplicationsKey == null)
                {
                    return;
                }

                foreach (var applicationKeyName in baseApplicationsKey.GetSubKeyNames())
                {
                    using var applicationKey = baseApplicationsKey.OpenSubKey(applicationKeyName);
                    if (applicationKey == null)
                    {
                        continue;
                    }

                    var displayName = applicationKey.GetValue("DisplayName") as string;
                    var installLocation = applicationKey.GetValue("InstallLocation") as string;

                    if (string.IsNullOrWhiteSpace(displayName))
                    {
                        continue;
                    }

                    var displayIcon = applicationKey.GetValue("DisplayIcon") as string;
                    var displayVersion = applicationKey.GetValue("DisplayVersion") as string;

                    installedApplications.TryAdd(displayName, new ApplicationModel
                    {
                        DisplayIcon = displayIcon,
                        DisplayName = displayName,
                        DisplayVersion = displayVersion,
                        InstallLocation = installLocation
                    });
                }
            }
        }

        private static IEnumerable<string> GetOpenWithList(string fileExtension, RegistryKey rootKey,
            string baseKeyName = "")
        {
            var results = new List<string>();

            baseKeyName = @$"{baseKeyName?.TrimEnd('\\')}\{fileExtension}\OpenWithList";

            using var registryKey = rootKey.OpenSubKey(baseKeyName);
            if (!(registryKey?.GetValue("MRUList") is string mruList))
            {
                return results;
            }

            foreach (var mru in mruList)
            {
                if (registryKey.GetValue(mru.ToString()) is string name)
                {
                    results.Add(name);
                }
            }

            return results;
        }

        private static IEnumerable<string> GetOpenWithProgids(string fileExtension, RegistryKey rootKey,
            string baseKeyName = "")
        {
            var results = new List<string>();

            baseKeyName = @$"{baseKeyName?.TrimEnd('\\')}\{fileExtension}\OpenWithProgids";

            using var registryKey = rootKey.OpenSubKey(baseKeyName);
            if (registryKey != null)
            {
                results.AddRange(registryKey.GetValueNames());
            }

            return results;
        }

        private static class Win32Api
        {
            public static string FindExecutable(string fileName)
            {
                var buffer = new StringBuilder(1024);
                var result = FindExecutableA(fileName, string.Empty, buffer);
                return result >= 32 ? buffer.ToString() : null;
            }

            public static string AssocQueryString(AssocF assocF, AssocStr association, string assocString)
            {
                var length = 0u;
                var ret = AssocQueryString(assocF, association, assocString, null, null, ref length);
                if (ret != 1)
                {
                    return null;
                }

                var builder = new StringBuilder((int) length);
                ret = AssocQueryString(assocF, association, assocString, null, builder, ref length);
                return ret != 0 ? null : builder.ToString();
            }

            [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
            private static extern long FindExecutableA(string lpFile, string lpDirectory, StringBuilder lpResult);

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