using System.Threading.Tasks;
using System.Timers;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Configuration;

namespace Camelot.Services.Drives
{
    public class DrivesUpdateService : IDrivesUpdateService
    {
        private readonly IMountedDriveService _mountedDriveService;
        private readonly IUnmountedDriveService _unmountedDriveService;

        private readonly Timer _timer;

        public DrivesUpdateService(
            IMountedDriveService mountedDriveService,
            IUnmountedDriveService unmountedDriveService,
            DriveServiceConfiguration configuration)
        {
            _mountedDriveService = mountedDriveService;
            _unmountedDriveService = unmountedDriveService;
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
            ReloadMountedDrives();
            await ReloadUnmountedDrivesAsync();
        }

        private void ReloadMountedDrives() => _mountedDriveService.ReloadMountedDrives();

        private Task ReloadUnmountedDrivesAsync() => _unmountedDriveService.ReloadUnmountedDrivesAsync();
    }
}