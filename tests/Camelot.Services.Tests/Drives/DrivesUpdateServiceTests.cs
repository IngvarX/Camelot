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

            var taskCompletionSource = new TaskCompletionSource<bool>();

            _autoMocker
                .Setup<IUnmountedDriveService>(m => m.ReloadUnmountedDrivesAsync())
                .Callback(() =>
                {
                    try
                    {
                        taskCompletionSource.SetResult(true);
                    }
                    catch
                    {
                        // ignore
                    }
                });

            var service = _autoMocker.CreateInstance<DrivesUpdateService>();

            await Task.Delay(1000);

            _autoMocker
                .Verify<IMountedDriveService>(m => m.ReloadMountedDrives(), Times.Never);
            _autoMocker
                .Verify<IUnmountedDriveService>(m => m.ReloadUnmountedDrivesAsync(), Times.Never);

            service.Start();

            var task = await Task.WhenAny(Task.Delay(2000), taskCompletionSource.Task);
            if (task != taskCompletionSource.Task)
            {
                taskCompletionSource.SetResult(false);
            }

            var result = await taskCompletionSource.Task;
            Assert.True(result);

            _autoMocker
                .Verify<IMountedDriveService>(m => m.ReloadMountedDrives(), Times.AtLeastOnce);
            _autoMocker
                .Verify<IUnmountedDriveService>(m => m.ReloadUnmountedDrivesAsync(), Times.AtLeastOnce);
        }
    }
}