using System.Threading.Tasks;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Configuration;
using Camelot.Services.Drives;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests.Drives
{
    public class DrivesUpdateServiceTests
    {
        private readonly AutoMocker _autoMocker;

        public DrivesUpdateServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task TestReload()
        {
            _autoMocker.Use(new DriveServiceConfiguration {DrivesListRefreshIntervalMs = 10});
            var service = _autoMocker.CreateInstance<DrivesUpdateService>();

            await Task.Delay(100);

            _autoMocker
                .Verify<IMountedDriveService>(m => m.ReloadMountedDrives(), Times.Never);
            _autoMocker
                .Verify<IUnmountedDriveService>(m => m.ReloadUnmountedDrivesAsync(), Times.Never);

            service.Start();

            await Task.Delay(100);

            _autoMocker
                .Verify<IMountedDriveService>(m => m.ReloadMountedDrives(), Times.AtLeastOnce);
            _autoMocker
                .Verify<IUnmountedDriveService>(m => m.ReloadUnmountedDrivesAsync(), Times.AtLeastOnce);
        }
    }
}