using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Environment.Models;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Windows.Tests;

public class WindowsMountedDriveServiceTests
{
    private readonly AutoMocker _autoMocker;

    public WindowsMountedDriveServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData("D", "mountvol", "D /p")]
    [InlineData("E", "mountvol", "E /p")]
    [InlineData("Z", "mountvol", "Z /p")]
    public void TestUnmount(string rootDirectory, string command, string arguments)
    {
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>());
        _autoMocker
            .Setup<IProcessService>(m => m.Run(command, arguments))
            .Verifiable();

        var service = _autoMocker.CreateInstance<WindowsMountedDriveService>();
        service.Unmount(rootDirectory);

        _autoMocker
            .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
    }

    [Fact]
    public async Task TestEject()
    {
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>());

        var service = _autoMocker.CreateInstance<WindowsMountedDriveService>();
        Task EjectAsync() => service.EjectAsync("C");

        await Assert.ThrowsAsync<NotSupportedException>(EjectAsync);
    }
}