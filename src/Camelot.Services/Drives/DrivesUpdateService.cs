using System;
using System.Threading.Tasks;
using System.Timers;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Configuration;
using Microsoft.Extensions.Logging;

namespace Camelot.Services.Drives;

public class DrivesUpdateService : IDrivesUpdateService
{
    private readonly IMountedDriveService _mountedDriveService;
    private readonly IUnmountedDriveService _unmountedDriveService;
    private readonly ILogger _logger;

    private readonly Timer _timer;

    public DrivesUpdateService(
        IMountedDriveService mountedDriveService,
        IUnmountedDriveService unmountedDriveService,
        ILogger logger,
        DriveServiceConfiguration configuration)
    {
        _mountedDriveService = mountedDriveService;
        _unmountedDriveService = unmountedDriveService;
        _logger = logger;
        _timer = CreateTimer(configuration);
    }

    public void Start() => _timer.Start();

    private Timer CreateTimer(DriveServiceConfiguration configuration)
    {
        var timer = new Timer(configuration.DrivesListRefreshIntervalMs);
        timer.Elapsed += TimerOnElapsed;

        return timer;
    }

    private async void TimerOnElapsed(object sender, ElapsedEventArgs e) => await ReloadDrivesAsync();

    private async Task ReloadDrivesAsync()
    {
        try
        {
            ReloadMountedDrives();
            await ReloadUnmountedDrivesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to update drives: {ex}");
        }
    }

    private void ReloadMountedDrives() => _mountedDriveService.ReloadMountedDrives();

    private Task ReloadUnmountedDrivesAsync() => _unmountedDriveService.ReloadUnmountedDrivesAsync();
}