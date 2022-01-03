using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Moq.AutoMock;
using Xunit;
using DriveInfo = Camelot.Services.Environment.Models.DriveInfo;

namespace Camelot.Services.AllPlatforms.Tests;

public class MountedDriveServiceTests
{
    private readonly AutoMocker _autoMocker;

    public MountedDriveServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestGetDrives()
    {
        var drives = GetDrives();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(drives);

        var driveService = _autoMocker.CreateInstance<MountedDriveService>();

        Assert.NotNull(driveService.MountedDrives);
        Assert.Equal(3, driveService.MountedDrives.Count);
    }

    [Fact]
    public void TestAddDrives()
    {
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>())
            .Verifiable();

        var driveService = _autoMocker.CreateInstance<MountedDriveService>();

        Assert.NotNull(driveService.MountedDrives);
        Assert.Empty(driveService.MountedDrives);

        var drives = GetDrives();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(drives);

        var isCallbackCalled = false;
        driveService.DriveAdded += (sender, args) => isCallbackCalled = true;

        driveService.ReloadMountedDrives();

        Assert.NotNull(driveService.MountedDrives);
        Assert.NotEmpty(driveService.MountedDrives);
        Assert.True(isCallbackCalled);
    }

    [Fact]
    public void TestRemoveDrives()
    {
        var drives = GetDrives();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(drives);

        var driveService = _autoMocker.CreateInstance<MountedDriveService>();

        Assert.NotNull(driveService.MountedDrives);
        Assert.NotEmpty(driveService.MountedDrives);

        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(Array.Empty<DriveInfo>())
            .Verifiable();

        var isCallbackCalled = false;
        driveService.DriveRemoved += (sender, args) => isCallbackCalled = true;

        driveService.ReloadMountedDrives();

        Assert.NotNull(driveService.MountedDrives);
        Assert.Empty(driveService.MountedDrives);
        Assert.True(isCallbackCalled);
    }

    [Fact]
    public void TestUpdateDrives()
    {
        var drives = GetDrives();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(drives);

        var driveService = _autoMocker.CreateInstance<MountedDriveService>();

        Assert.NotNull(driveService.MountedDrives);
        Assert.NotEmpty(driveService.MountedDrives);

        var callbackCalledCount = 0;
        driveService.DriveUpdated += (sender, args) => callbackCalledCount++;

        drives[0].FreeSpaceBytes++;
        driveService.ReloadMountedDrives();

        drives[1].FreeSpaceBytes++;
        driveService.ReloadMountedDrives();

        drives[2].Name = "newName";
        driveService.ReloadMountedDrives();

        Assert.NotNull(driveService.MountedDrives);
        Assert.NotEmpty(driveService.MountedDrives);
        Assert.Equal(3, callbackCalledCount);
    }

    [Theory]
    [InlineData("/home/a/b/c/text.txt", "/home/a/b/c")]
    [InlineData("/home/a/b/text.txt", "/home/a/b")]
    [InlineData("/home/a/text.txt", "/home/a")]
    public void TestGetFileDrive(string filePath, string rootDirectory)
    {
        var drives = GetDrives();
        _autoMocker
            .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
            .Returns(drives);

        var driveService = _autoMocker.CreateInstance<MountedDriveService>();

        var drive = driveService.GetFileDrive(filePath);

        Assert.NotNull(drive);
        Assert.NotNull(drive.Name);
        Assert.NotNull(drive.RootDirectory);

        Assert.Equal(rootDirectory, drive.RootDirectory);
    }

    private static DriveInfo[] GetDrives() =>
        new[]
        {
            new DriveInfo
            {
                RootDirectory = "/home/a",
                Name = "A name",
                TotalSpaceBytes = 42,
                FreeSpaceBytes = 21,
                DriveType = DriveType.Fixed
            },
            new DriveInfo
            {
                RootDirectory = "/home/a/b",
                Name = "B name",
                TotalSpaceBytes = 42,
                FreeSpaceBytes = 21,
                DriveType = DriveType.Fixed
            },
            new DriveInfo
            {
                RootDirectory = "/home/a/b/c",
                Name = "C name",
                TotalSpaceBytes = 42,
                FreeSpaceBytes = 21,
                DriveType = DriveType.Fixed
            },
            new DriveInfo
            {
                RootDirectory = "/home/a/b/c/d",
                Name = "D name",
                TotalSpaceBytes = 0,
                FreeSpaceBytes = 0,
                DriveType = DriveType.Fixed
            },
            new DriveInfo
            {
                RootDirectory = "/home/a/b/c/d",
                Name = "D name",
                TotalSpaceBytes = 42,
                FreeSpaceBytes = 21,
                DriveType = DriveType.Unknown
            },
            new DriveInfo
            {
                RootDirectory = "/home/a/b/c/d/e",
                Name = "E name",
                TotalSpaceBytes = 42,
                FreeSpaceBytes = 21,
                DriveType = DriveType.Ram
            }
        };

    private class MountedDriveService : MountedDriveServiceBase
    {
        public MountedDriveService(IEnvironmentDriveService environmentDriveService)
            : base(environmentDriveService)
        {

        }

        public override void Unmount(string driveRootDirectory) => throw new NotImplementedException();

        public override Task EjectAsync(string driveRootDirectory) => throw new NotImplementedException();
    }
}