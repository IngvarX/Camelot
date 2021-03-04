using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Environment.Models;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Windows.Tests
{
    public class WindowsMountedDriveServiceTests
    {
        private readonly AutoMocker _autoMocker;

        public WindowsMountedDriveServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public async Task TestEject()
        {
            _autoMocker
                .Setup<IEnvironmentDriveService, IReadOnlyList<DriveInfo>>(m => m.GetMountedDrives())
                .Returns(Array.Empty<DriveInfo>());

            var service = _autoMocker.CreateInstance<WindowsMountedDriveService>();
            Task EjectAsync() => service.EjectAsync("C");

            await Assert.ThrowsAsync<NotSupportedException>(EjectAsync);
        }
    }
}