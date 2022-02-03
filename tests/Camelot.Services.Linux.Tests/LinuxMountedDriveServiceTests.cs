using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Environment.Models;
using Camelot.Services.Linux.Interfaces;
using Camelot.Tests.Shared.Extensions;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests;

public class LinuxMountedDriveServiceTests
{
    private readonly AutoMocker _autoMocker;

    public LinuxMountedDriveServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData("/home/test", "umount", "/home/test")]
    [InlineData("/home/camelot", "umount", "/home/camelot")]
    [InlineData("/dev/sda1", "umount", "/dev/sda1")]
    public void TestUnmount(string drive, string command, string arguments)
    {
        _autoMocker
            .Setup<IProcessService>(m => m.Run(command, arguments))
            .Verifiable();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>());

        var service = _autoMocker.CreateInstance<LinuxMountedDriveService>();
        service.Unmount(drive);

        _autoMocker
            .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
    }

    [Fact]
    public void TestUnmountException()
    {
        _autoMocker
            .Setup<IProcessService>(m => m.Run(It.IsAny<string>(), It.IsAny<string>()))
            .Throws<InvalidOperationException>();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>());
        _autoMocker.MockLogError();

        var service = _autoMocker.CreateInstance<LinuxMountedDriveService>();
        service.Unmount("/media/camelot/test");

        _autoMocker.VerifyLogError(Times.Once());
    }

    [Theory]
    [InlineData("/home/test", "/dev/sdc1", "eject", "/dev/sdc1")]
    [InlineData("/home/camelot", "/dev/sdc2", "eject", "/dev/sdc2")]
    [InlineData("/home/sda1", "/dev/sdc3", "eject", "/dev/sdc3")]
    public async Task TestEject(string rootDirectory, string driveName, string command, string arguments)
    {
        _autoMocker
            .Setup<IProcessService>(m => m.Run(command, arguments))
            .Verifiable();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>());
        _autoMocker
            .Setup<IDriveNameService, Task<string>>(m => m.GetDriveNameAsync(rootDirectory))
            .ReturnsAsync(driveName);

        var service = _autoMocker.CreateInstance<LinuxMountedDriveService>();
        await service.EjectAsync(rootDirectory);

        _autoMocker
            .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
    }

    [Fact]
    public async Task TestEjectException()
    {
        _autoMocker
            .Setup<IProcessService>(m => m.Run(It.IsAny<string>(), It.IsAny<string>()))
            .Throws<InvalidOperationException>();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>());
        _autoMocker.MockLogError();

        var service = _autoMocker.CreateInstance<LinuxMountedDriveService>();
        await service.EjectAsync("/media/camelot/test");

        _autoMocker.VerifyLogError(Times.Once());
    }
}