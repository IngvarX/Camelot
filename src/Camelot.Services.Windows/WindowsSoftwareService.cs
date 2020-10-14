using System.Collections.Generic;
using System.Runtime.InteropServices;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Microsoft.Win32;

namespace Camelot.Services.Windows
{
    public class WindowsSoftwareService : ISoftwareService
    {
        public IEnumerable<SoftwareModel> GetAllInstalledSoftwares()
        {
            var installedSoftwares = new List<SoftwareModel>();

            var uninstalls = Registry.LocalMachine.OpenSubKey("SOFTWARE");
            if (RuntimeInformation.ProcessArchitecture == Architecture.X64 ||
                RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
            {
                uninstalls = uninstalls.OpenSubKey("WOW6432Node");
            }

            uninstalls = uninstalls
                .OpenSubKey("Microsoft")
                .OpenSubKey("Windows")
                .OpenSubKey("CurrentVersion")
                .OpenSubKey("Uninstall");

            foreach (var sofwareKeyName in uninstalls.GetSubKeyNames())
            {
                using var sofwareRegistryKey = uninstalls.OpenSubKey(sofwareKeyName);
                if (sofwareRegistryKey == null)
                {
                    continue;
                }

                var displayName = sofwareRegistryKey.GetValue("DisplayName") as string;
                if (string.IsNullOrWhiteSpace(displayName))
                {
                    continue;
                }

                var displayIcon = sofwareRegistryKey.GetValue("DisplayIcon") as string;
                var displayVersion = sofwareRegistryKey.GetValue("DisplayVersion") as string;
                var installLocation = sofwareRegistryKey.GetValue("InstallLocation") as string;

                installedSoftwares.Add(new SoftwareModel
                {
                    DisplayIcon = displayIcon,
                    DisplayName = displayName,
                    DisplayVersion = displayVersion,
                    InstallLocation = installLocation
                });
            }

            return installedSoftwares;
        }
    }
}
