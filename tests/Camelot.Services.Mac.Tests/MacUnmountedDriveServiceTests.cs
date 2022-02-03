using System;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Mac.Configuration;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Mac.Tests;

public class MacUnmountedDriveServiceTests
{
    private readonly AutoMocker _autoMocker;

    public MacUnmountedDriveServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData("/dev/disk1", "mountDisk /dev/disk1")]
    [InlineData("/dev/disk2", "mountDisk /dev/disk2")]
    public void TestMount(string driveName, string command)
    {
        _autoMocker
            .Setup<IProcessService>(m => m.Run("diskutil", command))
            .Verifiable();

        var service = _autoMocker.CreateInstance<MacUnmountedDriveService>();
        service.Mount(driveName);

        _autoMocker
            .Verify<IProcessService>(m => m.Run("diskutil", command), Times.Once);
    }

    [Fact]
    public async Task TestGetUnmountedDrivesDisabled()
    {
        var service = _autoMocker.CreateInstance<MacUnmountedDriveService>();

        await service.ReloadUnmountedDrivesAsync();
        var drives = service.UnmountedDrives;

        Assert.NotNull(drives);
        Assert.Empty(drives);
    }

    [Fact]
    public async Task TestGetUnmountedDrivesEnabledException()
    {
        var configuration = new UnmountedDrivesConfiguration
        {
            IsEnabled = true
        };
        _autoMocker.Use(configuration);
        _autoMocker
            .Setup<IProcessService, Task<string>>(m =>
                m.ExecuteAndGetOutputAsync("diskutil", "list"))
            .ThrowsAsync(new Exception());

        var service = _autoMocker.CreateInstance<MacUnmountedDriveService>();

        await service.ReloadUnmountedDrivesAsync();
        var drives = service.UnmountedDrives;

        Assert.NotNull(drives);
        Assert.Empty(drives);
    }

    [Theory]
    [InlineData("/dev/disk0", "Filesystem\n/dev/disk0s1", new string[] {}, new string[] {})]
    [InlineData("/dev/disk1\ntest\ntest2", "Filesystem\n", new[] {"disk1"}, new[] {"/dev/disk1"})]
    [InlineData("/dev/disk1\ntest\n/dev/disk2", "Filesystem\n/dev/disk1s1", new[] {"disk2"}, new[] {"/dev/disk2"})]
    [InlineData("/dev/disk1\n\n\n/test/\n/dev/disk2", "Filesystem Size\n/dev/disk1s1 150G", new[] {"disk2"}, new[] {"/dev/disk2"})]
    public async Task TestGetUnmountedDrivesEnabled(string listOutput, string dfOutput,
        string[] names, string[] fullNames)
    {
        var configuration = new UnmountedDrivesConfiguration
        {
            IsEnabled = true
        };
        _autoMocker.Use(configuration);
        _autoMocker
            .Setup<IEnvironmentService, string>(m => m.NewLine)
            .Returns("\n");
        _autoMocker
            .Setup<IProcessService, Task<string>>(m =>
                m.ExecuteAndGetOutputAsync("diskutil", "list"))
            .ReturnsAsync(listOutput);
        _autoMocker
            .Setup<IProcessService, Task<string>>(m =>
                m.ExecuteAndGetOutputAsync("df", "-Hl"))
            .ReturnsAsync(dfOutput);

        var service = _autoMocker.CreateInstance<MacUnmountedDriveService>();

        await service.ReloadUnmountedDrivesAsync();
        var drives = service.UnmountedDrives;

        Assert.NotNull(drives);

        var actualNames = drives.Select(d => d.Name).ToArray();
        Assert.Equal(names, actualNames);

        var actualFullNames = drives.Select(d => d.FullName).ToArray();
        Assert.Equal(fullNames, actualFullNames);
    }
}