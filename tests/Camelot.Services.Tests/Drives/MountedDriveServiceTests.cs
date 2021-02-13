using System;
using System.IO;
using System.Reflection;
using Camelot.Services.Drives;
using Camelot.Services.Environment.Interfaces;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests.Drives
{
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
            var drives = DriveInfo.GetDrives();
            _autoMocker
                .Setup<IEnvironmentDriveService, DriveInfo[]>(m => m.GetMountedDrives())
                .Returns(drives);

            var driveService = _autoMocker.CreateInstance<MountedDriveService>();

            Assert.NotNull(driveService.MountedDrives);
            Assert.NotEmpty(driveService.MountedDrives);
        }

        [Fact]
        public void TestAddDrives()
        {
            _autoMocker
                .Setup<IEnvironmentDriveService, DriveInfo[]>(m => m.GetMountedDrives())
                .Returns(Array.Empty<DriveInfo>())
                .Verifiable();

            var driveService = _autoMocker.CreateInstance<MountedDriveService>();

            Assert.NotNull(driveService.MountedDrives);
            Assert.Empty(driveService.MountedDrives);

            var drives = DriveInfo.GetDrives();
            _autoMocker
                .Setup<IEnvironmentDriveService, DriveInfo[]>(m => m.GetMountedDrives())
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
            var drives = DriveInfo.GetDrives();
            _autoMocker
                .Setup<IEnvironmentDriveService, DriveInfo[]>(m => m.GetMountedDrives())
                .Returns(drives);

            var driveService = _autoMocker.CreateInstance<MountedDriveService>();

            Assert.NotNull(driveService.MountedDrives);
            Assert.NotEmpty(driveService.MountedDrives);

            _autoMocker
                .Setup<IEnvironmentDriveService, DriveInfo[]>(m => m.GetMountedDrives())
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
        public void TestGetFileDrive()
        {
            var drives = DriveInfo.GetDrives();
            _autoMocker
                .Setup<IEnvironmentDriveService, DriveInfo[]>(m => m.GetMountedDrives())
                .Returns(drives);

            var driveService = _autoMocker.CreateInstance<MountedDriveService>();

            var filePath = Assembly.GetEntryAssembly().Location;
            var drive = driveService.GetFileDrive(filePath);

            Assert.NotNull(drive);
            Assert.NotNull(drive.Name);
            Assert.NotNull(drive.RootDirectory);

            Assert.NotEmpty(drive.Name);
            Assert.NotEmpty(drive.RootDirectory);
            Assert.True(drive.FreeSpaceBytes > 0);
            Assert.True(drive.TotalSpaceBytes > 0);
        }
    }
}