using System;
using System.Threading.Tasks;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsUnmountedDriveServiceTests
    {
        private const string DriveName = "Drive";

        [Fact]
        public async Task TestGetUnmountedDrives()
        {
            var service = new WindowsUnmountedDriveService();
            await service.ReloadUnmountedDrivesAsync();
            var models = service.UnmountedDrives;

            Assert.NotNull(models);
            Assert.Empty(models);
        }

        [Fact]
        public void TestMount()
        {
            var service = new WindowsUnmountedDriveService();

            void Mount() => service.Mount(DriveName);

            Assert.Throws<InvalidOperationException>(Mount);
        }
    }
}