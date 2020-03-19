using System;
using Camelot.Services.Enums;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class FileOpeningService : IFileOpeningService
    {
        private readonly IProcessService _processService;
        private readonly IPlatformService _platformService;

        public FileOpeningService(
            IProcessService processService,
            IPlatformService platformService)
        {
            _processService = processService;
            _platformService = platformService;
        }

        public void Open(string file)
        {
            var platform = _platformService.GetPlatform();
            if (platform is Platform.Windows)
            {
                _processService.Run(file);

                return;
            }

            var command = GetCommand(platform);
            const string arguments = "\"{file}\"";

            _processService.Run(command, arguments);
        }

        private static string GetCommand(Platform platform)
        {
            return platform switch
            {
                Platform.Linux => "xdg-open",
                Platform.MacOs => "open",
                _ => throw new NotSupportedException("Unsupported platform")
            };
        }
    }
}