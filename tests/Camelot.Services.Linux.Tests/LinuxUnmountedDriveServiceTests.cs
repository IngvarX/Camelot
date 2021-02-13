using System;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Configuration;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class LinuxUnmountedDriveServiceTests
    {
        private readonly AutoMocker _autoMocker;

        public LinuxUnmountedDriveServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task TestGetUnmountedDrivesDisabled()
        {
            var service = _autoMocker.CreateInstance<LinuxUnmountedDriveService>();

            await service.ReloadUnmountedDrivesAsync();
            var drives = service.UnmountedDrives;

            Assert.NotNull(drives);
            Assert.Empty(drives);
        }

        [Theory]
        [InlineData("sda1 /mnt/test", new string[] {}, new string[] {})]
        [InlineData("sda1 ", new[] {"sda1"}, new[] {"/dev/sda1"})]
        [InlineData("loop ", new string[] {}, new string[] {})]
        [InlineData("sda \nsdc2 ", new[] {"sdc2"}, new[] {"/dev/sdc2"})]
        [InlineData("\n\n \n", new string[] {}, new string[] {})]
        [InlineData("sd \n,s", new string[] {}, new string[] {})]
        [InlineData("sda1 \nsdc \nsdc2 \nsdc3 /mnt/test",  new[] {"sda1", "sdc2"}, new[] {"/dev/sda1", "/dev/sdc2"})]
        public async Task TestGetUnmountedDrivesEnabled(string output, string[] names, string[] fullNames)
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
                    m.ExecuteAndGetOutputAsync("lsblk", "--noheadings --raw -o NAME,MOUNTPOINT"))
                .ReturnsAsync(output);

            var service = _autoMocker.CreateInstance<LinuxUnmountedDriveService>();

            await service.ReloadUnmountedDrivesAsync();
            var drives = service.UnmountedDrives;

            Assert.NotNull(drives);

            var actualNames = drives.Select(d => d.Name).ToArray();
            Assert.Equal(names, actualNames);

            var actualFullNames = drives.Select(d => d.FullName).ToArray();
            Assert.Equal(fullNames, actualFullNames);
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
                    m.ExecuteAndGetOutputAsync("lsblk", "--noheadings --raw -o NAME,MOUNTPOINT"))
                .ThrowsAsync(new Exception());

            var service = _autoMocker.CreateInstance<LinuxUnmountedDriveService>();

            await service.ReloadUnmountedDrivesAsync();
            var drives = service.UnmountedDrives;

            Assert.NotNull(drives);
            Assert.Empty(drives);
        }

        [Theory]
        [InlineData("/dev/sda1", "mount -b /dev/sda1")]
        [InlineData("/dev/sdc2", "mount -b /dev/sdc2")]
        public void TestMount(string driveName, string command)
        {
            _autoMocker
                .Setup<IProcessService>(m => m.Run("udisksctl", command))
                .Verifiable();

            var service = _autoMocker.CreateInstance<LinuxUnmountedDriveService>();
            service.Mount(driveName);

            _autoMocker
                .Verify<IProcessService>(m => m.Run("udisksctl", command), Times.Once);
        }
    }
}