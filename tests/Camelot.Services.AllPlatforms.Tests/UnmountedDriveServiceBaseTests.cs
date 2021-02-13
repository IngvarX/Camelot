using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Xunit;

namespace Camelot.Services.AllPlatforms.Tests
{
    public class UnmountedDriveServiceBaseTests
    {
        [Fact]
        public async Task TestAddDrive()
        {
            var list = new List<UnmountedDriveModel>();
            var service = new UnmountedDriveService(list);

            Assert.NotNull(service.UnmountedDrives);
            Assert.Empty(service.UnmountedDrives);

            var model = new UnmountedDriveModel();
            list.Add(model);

            var isCallbackCalled = false;
            service.DriveAdded += (sender, args) => isCallbackCalled = args.UnmountedDriveModel == model;

            await service.ReloadUnmountedDrivesAsync();

            Assert.NotNull(service.UnmountedDrives);
            Assert.Single(service.UnmountedDrives);
            Assert.Equal(model, service.UnmountedDrives.Single());
            Assert.True(isCallbackCalled);
        }

        [Fact]
        public async Task TestRemoveDrive()
        {
            var model = new UnmountedDriveModel();
            var list = new List<UnmountedDriveModel>{model};
            var service = new UnmountedDriveService(list);

            await service.ReloadUnmountedDrivesAsync();

            var isCallbackCalled = false;
            service.DriveRemoved += (sender, args) => isCallbackCalled = args.UnmountedDriveModel == model;
            list.Remove(model);

            await service.ReloadUnmountedDrivesAsync();

            Assert.NotNull(service.UnmountedDrives);
            Assert.Empty(service.UnmountedDrives);
            Assert.True(isCallbackCalled);
        }

        private class UnmountedDriveService : UnmountedDriveServiceBase
        {
            private readonly List<UnmountedDriveModel> _drives;

            public UnmountedDriveService(
                List<UnmountedDriveModel> drives)
            {
                _drives = drives;
            }

            public override void Mount(string drive) => throw new System.NotImplementedException();

            protected override Task<IReadOnlyList<UnmountedDriveModel>> GetUnmountedDrivesAsync() =>
                Task.FromResult<IReadOnlyList<UnmountedDriveModel>>(_drives);
        }
    }
}