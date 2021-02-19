using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacMountedDriveService : MountedDriveServiceBase
    {
        private const string UnmountDriveCommand = "diskutil";
        private const string UnmountDriveArguments = "unmount {0}";

        private readonly IProcessService _processService;

        public MacMountedDriveService(
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