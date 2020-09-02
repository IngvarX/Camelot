using System.IO;
using System.Linq;
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
                .Setup(m => m.GetUnmountedDrives())
                .Returns(Enumerable.Empty<UnmountedDriveModel>());

            var configuration = new DriveServiceConfiguration
            {
                DrivesListRefreshIntervalMs = 10
            };
            var driveService = new DriveService(envDriveServiceMock.Object, unmountedDriveServiceMock.Object,
                configuration);
            var drivesModels = driveService.MountedDrives;

            Assert.NotNull(drivesModels);
            Assert.NotEmpty(drivesModels);
        }
    }
}