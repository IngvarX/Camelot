using System.Collections.Generic;
using System.Runtime.InteropServices;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Microsoft.Win32;

namespace Camelot.Services.Windows
{
    public class WindowsSoftwareService : ISoftwareService
    {
        private const string RegkeyInstalledSoftwareX32 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string RegkeyInstalledSoftwareX64 = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        public IEnumerable<SoftwareModel> GetAllInstalledSoftwares()
        {
            var installedSoftwares = new List<SoftwareModel>();

            using var softwareX32Key = Registry.LocalMachine.OpenSubKey(RegkeyInstalledSoftwareX32);
            AddSoftwares(softwareX32Key);

            if (RuntimeInformation.ProcessArchitecture == Architecture.X64 ||
                RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                using var softwareX64Key = Registry.LocalMachine.OpenSubKey(RegkeyInstalledSoftwareX64);
                AddSoftwares(softwareX64Key);
            }

            return installedSoftwares;

            void AddSoftwares(RegistryKey softwareKey)
            {
                foreach (var softwareKeyName in softwareKey.GetSubKeyNames())
                {
                    using var softwareRegistryKey = softwareKey.OpenSubKey(softwareKeyName);
                    if (softwareRegistryKey == null)
                    {
                        continue;
                    }

                    var displayName = softwareRegistryKey.GetValue("DisplayName") as string;
                    var installLocation = softwareRegistryKey.GetValue("InstallLocation") as string;

                    if (string.IsNullOrWhiteSpace(displayName) ||
                        string.IsNullOrWhiteSpace(installLocation))
                    {
                        continue;
                    }

                    var displayIcon = softwareRegistryKey.GetValue("DisplayIcon") as string;
                    var displayVersion = softwareRegistryKey.GetValue("DisplayVersion") as string;

                    installedSoftwares.Add(new SoftwareModel
                    {
                        DisplayIcon = displayIcon,
                        DisplayName = displayName,
                        DisplayVersion = displayVersion,
                        InstallLocation = installLocation
                    });
                }
            }
        }
    }
}
