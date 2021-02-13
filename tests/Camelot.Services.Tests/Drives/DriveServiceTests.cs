using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Configuration;
using Camelot.Services.Drives;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests.Drives
{
    public class DriveServiceTests
    {
        private readonly AutoMocker _autoMocker;

        public DriveServiceTests()
        {
            _autoMocker = new AutoMocker();
        }
        //
        // [Fact]
        // public void TestGetDrives()
        // {
        //     var drives = DriveInfo.GetDrives();
        //     var envDriveServiceMock = new Mock<IEnvironmentDriveService>();
        //     envDriveServiceMock
        //         .Setup(m => m.GetMountedDrives())
        //         .Returns(drives);
        //     var unmountedDriveServiceMock = new Mock<IUnmountedDriveService>();
        //     unmountedDriveServiceMock
        //         .Setup(m => m.GetUnmountedDrivesAsync())
        //         .ReturnsAsync(new []
        //         {
        //             new UnmountedDriveModel()
        //         });
        //
        //     var configuration = new DriveServiceConfiguration
        //     {
        //         DrivesListRefreshIntervalMs = 10_000
        //     };
        //     var driveService = new MountedDriveService(envDriveServiceMock.Object, unmountedDriveServiceMock.Object,
        //         configuration);
        //
        //     Assert.NotNull(driveService.MountedDrives);
        //     Assert.NotNull(driveService.UnmountedDrives);
        //     Assert.NotEmpty(driveService.MountedDrives);
        //     Assert.NotEmpty(driveService.UnmountedDrives);
        // }
        //
        // [Fact]
        // public async Task TestReloadDrives()
        // {
        //     var envDriveServiceMock = new Mock<IEnvironmentDriveService>();
        //     envDriveServiceMock
        //         .Setup(m => m.GetMountedDrives())
        //         .Returns(Array.Empty<DriveInfo>())
        //         .Verifiable();
        //     var unmountedDriveServiceMock = new Mock<IUnmountedDriveService>();
        //     unmountedDriveServiceMock
        //         .Setup(m => m.GetUnmountedDrivesAsync())
        //         .ReturnsAsync(new []
        //         {
        //             new UnmountedDriveModel
        //             {
        //                 FullName = "Test1"
        //             }
        //         })
        //         .Verifiable();
        //
        //     var configuration = new DriveServiceConfiguration
        //     {
        //         DrivesListRefreshIntervalMs = 10
        //     };
        //     var driveService = new MountedDriveService(envDriveServiceMock.Object, unmountedDriveServiceMock.Object,
        //         configuration);
        //
        //     await Task.Delay(300);
        //
        //     Assert.NotNull(driveService.MountedDrives);
        //     Assert.NotNull(driveService.UnmountedDrives);
        //
        //     envDriveServiceMock
        //         .Verify(m => m.GetMountedDrives(), Times.AtLeast(2));
        //     unmountedDriveServiceMock
        //         .Verify(m => m.GetUnmountedDrivesAsync(), Times.AtLeast(2));
        //
        //     var isCallbackCalled = false;
        //     driveService.DrivesListChanged += (sender, args) => isCallbackCalled = true;
        //
        //     unmountedDriveServiceMock
        //         .Setup(m => m.GetUnmountedDrivesAsync())
        //         .ReturnsAsync(new []
        //         {
        //             new UnmountedDriveModel
        //             {
        //                 FullName = "Test2"
        //             }
        //         })
        //         .Verifiable();
        //
        //     await Task.Delay(300);
        //
        //     Assert.True(isCallbackCalled);
        // }

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