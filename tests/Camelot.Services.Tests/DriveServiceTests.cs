using System.IO;
using Camelot.Services.Configuration;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class DriveServiceTests
    {
        [Fact]
        public void TestGetDrives()
        {
            var drives = DriveInfo.GetDrives();
            var envDriveServiceMock = new Mock<IEnvironmentDriveService>();
            envDriveServiceMock
                .Setup(m => m.GetDrives())
                .Returns(drives);

            var configuration = new DriveServiceConfiguration
            {
                DrivesListRefreshIntervalMs = 10
            };
            var driveService = new DriveService(envDriveServiceMock.Object, configuration);
            var drivesModels = driveService.Drives;

            Assert.NotNull(drivesModels);
            Assert.NotEmpty(drivesModels);
        }
    }
}