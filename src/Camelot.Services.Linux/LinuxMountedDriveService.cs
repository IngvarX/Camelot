using System;
using System.Threading.Tasks;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Interfaces;
using Microsoft.Extensions.Logging;

namespace Camelot.Services.Linux;

public class LinuxMountedDriveService : MountedDriveServiceBase
{
    private const string UnmountDriveCommand = "umount";
    private const string UnmountDriveArguments = "{0}";
    private const string EjectDriveCommand = "eject";
    private const string EjectDriveArguments = "{0}";

    private readonly IProcessService _processService;
    private readonly IDriveNameService _driveNameService;
    private readonly ILogger _logger;

    public LinuxMountedDriveService(
        IEnvironmentDriveService environmentDriveService,
        IProcessService processService,
        IDriveNameService driveNameService,
        ILogger logger)
        : base(environmentDriveService)
    {
        _processService = processService;
        _driveNameService = driveNameService;
        _logger = logger;
    }

    public override void Unmount(string driveRootDirectory)
    {
        var arguments = string.Format(UnmountDriveArguments, driveRootDirectory);

        try
        {
            _processService.Run(UnmountDriveCommand, arguments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during unmount");
        }
    }

    public override async Task EjectAsync(string driveRootDirectory)
    {
        var driveName = await _driveNameService.GetDriveNameAsync(driveRootDirectory);
        var arguments = string.Format(EjectDriveArguments, driveName);

        try
        {
            _processService.Run(EjectDriveCommand, arguments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during eject");
        }
    }
}