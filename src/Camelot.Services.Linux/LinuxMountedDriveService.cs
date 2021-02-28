using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Linux
{
    public class LinuxMountedDriveService : MountedDriveServiceBase
    {
        private const string UnmountDriveCommand = "umount";
        private const string UnmountDriveArguments = "{0}";
        private const string EjectDriveCommand = "eject";
        private const string EjectDriveArguments = "{0}";

        private readonly IProcessService _processService;

        public LinuxMountedDriveService(
            IEnvironmentDriveService environmentDriveService,
            IProcessService processService)
            : base(environmentDriveService)
        {
            _processService = processService;
        }

        public override void Unmount(string driveRootDirectory)
        {
            var arguments = string.Format(UnmountDriveArguments, driveRootDirectory);

            _processService.Run(UnmountDriveCommand, arguments);
        }

        public override void Eject(string driveRootDirectory)
        {
            var arguments = string.Format(EjectDriveArguments, driveRootDirectory);

            _processService.Run(EjectDriveCommand, arguments);
        }
    }
}