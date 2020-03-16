using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class FileOpeningService : IFileOpeningService
    {
        public void Open(string file)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(file);
                return;
            }

            var command = GetCommand();
            var processStartInfo = new ProcessStartInfo(command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                Arguments = $"\"{file}\""
            };
            var process = new Process { StartInfo = processStartInfo };

            process.Start();
        }

        private static string GetCommand()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "xdg-open";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "open";
            }

            throw new NotSupportedException("Unsupported platform");
        }
    }
}