using System;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Interfaces;
using Mono.Unix;

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

            if (CheckIfExecutable(resource))
            {
                _processService.Run(resource);

                return;
            }

            var command = GetCommand(platform);
            var arguments = $"\"{resource}\"";

            _processService.Run(command, arguments);
        }

        private static bool CheckIfExecutable(string resource)
        {
            var fileInfo = new UnixFileInfo(resource);
            var permissions = fileInfo.FileAccessPermissions;

            return CheckIfFlagIsSet(permissions, FileAccessPermissions.GroupExecute)
                   || CheckIfFlagIsSet(permissions, FileAccessPermissions.UserExecute)
                   || CheckIfFlagIsSet(permissions, FileAccessPermissions.OtherExecute);
        }

        private static bool CheckIfFlagIsSet(FileAccessPermissions permissions, FileAccessPermissions flag) =>
            (permissions & flag) == flag;

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