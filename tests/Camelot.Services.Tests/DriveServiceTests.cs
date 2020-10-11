using System;
using System.IO;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Configuration;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class DriveServiceTests
    {
        [Fact]
        public void TestGetMountedDrives()
        {
            var drives = DriveInfo.GetDrives();
            var envDriveServiceMock = new Mock<IEnvironmentDriveService>();
            envDriveServiceMock
                .Setup(m => m.GetMountedDrives())
                .Returns(drives);
            var unmountedDriveServiceMock = new Mock<IUnmountedDriveService>();
            unmountedDriveServiceMock
                .Setup(m => m.GetUnmountedDrivesAsync())
                .ReturnsAsync(new []
                {
                    new UnmountedDriveModel()
                });

            var configuration = new DriveServiceConfiguration
            {
                DrivesListRefreshIntervalMs = 10
            };
            var driveService = new DriveService(envDriveServiceMock.Object, unmountedDriveServiceMock.Object,
                configuration);

            Assert.NotNull(driveService.MountedDrives);
            Assert.NotNull(driveService.UnmountedDrives);
            Assert.NotEmpty(driveService.MountedDrives);
            Assert.NotEmpty(driveService.UnmountedDrives);
        }

        [Fact]
        public async Task TestReloadDrives()
        {
            var envDriveServiceMock = new Mock<IEnvironmentDriveService>();
            envDriveServiceMock
                .Setup(m => m.GetMountedDrives())
                .Returns(Array.Empty<DriveInfo>())
                .Verifiable();
            var unmountedDriveServiceMock = new Mock<IUnmountedDriveService>();
            unmountedDriveServiceMock
                .Setup(m => m.GetUnmountedDrivesAsync())
                .ReturnsAsync(new []
                {
                    new UnmountedDriveModel
                    {
                        FullName = "Test1"
                    }
                })
                .Verifiable();

            var configuration = new DriveServiceConfiguration
            {
                DrivesListRefreshIntervalMs = 10
            };
            var driveService = new DriveService(envDriveServiceMock.Object, unmountedDriveServiceMock.Object,
                configuration);

            await Task.Delay(100);

            Assert.NotNull(driveService.MountedDrives);
            Assert.NotNull(driveService.UnmountedDrives);

            envDriveServiceMock
                .Verify(m => m.GetMountedDrives(), Times.AtLeast(2));
            unmountedDriveServiceMock
                .Verify(m => m.GetUnmountedDrivesAsync(), Times.AtLeast(2));

            var isCallbackCalled = false;
            driveService.DrivesListChanged += (sender, args) => isCallbackCalled = true;

            unmountedDriveServiceMock
                .Setup(m => m.GetUnmountedDrivesAsync())
                .ReturnsAsync(new []
                {
                    new UnmountedDriveModel
                    {
                        FullName = "Test2"
                    }
                })
                .Verifiable();

            await Task.Delay(50);

            Assert.True(isCallbackCalled);
        }
    }
}