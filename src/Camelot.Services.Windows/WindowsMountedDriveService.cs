using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Windows
{
    public class WindowsMountedDriveService : MountedDriveServiceBase
    {
        private const string UnmountDriveCommand = "mountvol";
        private const string UnmountDriveArguments = "{0} /p";

        private readonly IProcessService _processService;

        public WindowsMountedDriveService(
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