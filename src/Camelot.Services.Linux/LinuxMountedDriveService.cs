using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Linux
{
    public class LinuxMountedDriveService : MountedDriveServiceBase
    {
        private const string UnmountDriveCommand = "umount";
        private const string UnmountDriveArguments = "{0}";

        private readonly IProcessService _processService;

        public LinuxMountedDriveService(
            IEnvironmentDriveService environmentDriveService,
            IProcessService processService)
            : base(environmentDriveService)
        {
            _processService = processService;
        }

        public override void Unmount(string drive)
        {
            var arguments = string.Format(UnmountDriveArguments, drive);

            _processService.Run(UnmountDriveCommand, arguments);
        }
    }
}