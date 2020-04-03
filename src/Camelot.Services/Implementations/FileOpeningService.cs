using System;
using Camelot.Services.Enums;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class ResourceOpeningService : IResourceOpeningService
    {
        private readonly IProcessService _processService;
        private readonly IPlatformService _platformService;

        public ResourceOpeningService(
            IProcessService processService,
            IPlatformService platformService)
        {
            _processService = processService;
            _platformService = platformService;
        }

        public void Open(string resource)
        {
            var platform = _platformService.GetPlatform();
            if (platform is Platform.Windows)
            {
                _processService.Run(resource);

                return;
            }

            var command = GetCommand(platform);
            var arguments = $"\"{resource}\"";

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