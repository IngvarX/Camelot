using Camelot.Services.Abstractions;
using Camelot.Services.Environment.Interfaces;
using Mono.Unix;

namespace Camelot.Services.Linux
{
    public class LinuxResourceOpeningService : IResourceOpeningService
    {
        private readonly IProcessService _processService;

        public LinuxResourceOpeningService(
            IProcessService processService)
        {
            _processService = processService;
        }

        public void Open(string resource)
        {
            if (CheckIfExecutable(resource))
            {
                _processService.Run(resource);

                return;
            }

            const string command = "xdg-open";
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
    }
}