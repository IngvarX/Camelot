using System.Threading.Tasks;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Mac;

public class MacMountedDriveService : MountedDriveServiceBase
{
    private const string UnmountDriveCommand = "diskutil";
    private const string UnmountDriveArguments = "unmount \"{0}\"";
    private const string EjectDriveCommand = "diskutil";
    private const string EjectDriveArguments = "eject \"{0}\"";

    private readonly IProcessService _processService;

    public MacMountedDriveService(
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

    public override Task EjectAsync(string driveRootDirectory)
    {
        var arguments = string.Format(EjectDriveArguments, driveRootDirectory);

        _processService.Run(EjectDriveCommand, arguments);

        return Task.CompletedTask;
    }
}