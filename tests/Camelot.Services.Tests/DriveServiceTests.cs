using System.IO;
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
            var drives = new[]
            {
                new DriveInfo("A"),
                new DriveInfo("B"),
                new DriveInfo("C"),
            };
            var envDriveServiceMock = new Mock<IEnvironmentDriveService>();
            envDriveServiceMock
                .Setup(m => m.GetDrives())
                .Returns(drives);

            var driveService = new DriveService(envDriveServiceMock.Object);
            var drivesModels = driveService.GetDrives();

            Assert.NotNull(drivesModels);
            Assert.Equal(drives.Length, drivesModels.Count);
        }
    }
}