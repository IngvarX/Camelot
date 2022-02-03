using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Environment.Models;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Mac.Tests;

public class MacMountedDriveServiceTests
{
    private readonly AutoMocker _autoMocker;

    public MacMountedDriveServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData("/home/test", "diskutil", "unmount \"/home/test\"")]
    [InlineData("/home/camelot", "diskutil", "unmount \"/home/camelot\"")]
    [InlineData("/dev/disk1", "diskutil", "unmount \"/dev/disk1\"")]
    public void TestUnmount(string drive, string command, string arguments)
    {
        _autoMocker
            .Setup<IProcessService>(m => m.Run(command, arguments))
            .Verifiable();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>());

        var service = _autoMocker.CreateInstance<MacMountedDriveService>();
        service.Unmount(drive);

        _autoMocker
            .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
    }

    [Theory]
    [InlineData("/home/test", "diskutil", "eject \"/home/test\"")]
    [InlineData("/home/camelot", "diskutil", "eject \"/home/camelot\"")]
    [InlineData("/dev/disk1", "diskutil", "eject \"/dev/disk1\"")]
    public async Task TestEject(string drive, string command, string arguments)
    {
        _autoMocker
            .Setup<IProcessService>(m => m.Run(command, arguments))
            .Verifiable();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>());

        var service = _autoMocker.CreateInstance<MacMountedDriveService>();
        await service.EjectAsync(drive);

        _autoMocker
            .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
    }
}