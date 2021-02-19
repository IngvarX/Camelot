using System;
using System.Collections.Generic;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Environment.Models;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Linux.Tests
{
    public class LinuxMountedDriveServiceTests
    {
        private readonly AutoMocker _autoMocker;

        public LinuxMountedDriveServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData("/home/test", "umount", "/home/test")]
        [InlineData("/home/camelot", "umount", "/home/camelot")]
        [InlineData("/dev/sda1", "umount", "/dev/sda1")]
        public void TestUnmount(string drive, string command, string arguments)
        {
            _autoMocker
                .Setup<IProcessService>(m => m.Run(command, arguments))
                .Verifiable();
            _autoMocker
                .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
                .Returns(Array.Empty<DriveInfo>());

            var service = _autoMocker.CreateInstance<LinuxMountedDriveService>();
            service.Unmount(drive);

            _autoMocker
                .Verify<IProcessService>(m => m.Run(command, arguments), Times.Once);
        }
    }
}