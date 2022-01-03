using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Configuration;
using Camelot.Services.Drives;
using Camelot.Tests.Shared.Extensions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests.Drives;

public class DrivesUpdateServiceTests
{
    private readonly AutoMocker _autoMocker;

    public DrivesUpdateServiceTests()
    {
        _autoMocker = new AutoMocker();
        _autoMocker.Use(GetConfig());
    }

    [Fact]
    public async Task TestReload()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        var callsCount = 0;

        _autoMocker
            .Setup<IUnmountedDriveService>(m => m.ReloadUnmountedDrivesAsync())
            .Callback(() =>
            {
                if (Interlocked.Increment(ref callsCount) == 1)
                {
                    taskCompletionSource.SetResult(true);
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

    [Fact]
    public async Task TestReloadWithException()
    {
        _autoMocker.MockLogError();
        _autoMocker
            .Setup<IMountedDriveService>(m => m.ReloadMountedDrives())
            .Throws(new InvalidOperationException());

        var service = _autoMocker.CreateInstance<DrivesUpdateService>();
        service.Start();

        await Task.Delay(1500);

        _autoMocker
            .Verify<IMountedDriveService>(m => m.ReloadMountedDrives(), Times.AtLeastOnce);
        _autoMocker
            .Verify<IUnmountedDriveService>(m => m.ReloadUnmountedDrivesAsync(), Times.Never);
        _autoMocker.VerifyLogError(Times.AtLeastOnce());
    }

    private static DriveServiceConfiguration GetConfig() =>
        new DriveServiceConfiguration {DrivesListRefreshIntervalMs = 10};
}