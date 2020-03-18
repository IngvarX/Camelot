using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class FileOpeningService : IFileOpeningService
    {
        private readonly IProcessService _processService;

        public FileOpeningService(IProcessService processService)
        {
            _processService = processService;
        }

        public void Open(string file)
        {
            // TODO: to service
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _processService.Run(file);
                return;
            }

            var command = GetCommand();
            const string arguments = "\"{file}\"";

            _processService.Run(command, arguments);
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