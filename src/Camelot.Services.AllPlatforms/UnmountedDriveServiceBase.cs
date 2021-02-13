using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.AllPlatforms
{
    public abstract class UnmountedDriveServiceBase : IUnmountedDriveService
    {
        private readonly List<UnmountedDriveModel> _unmountedDrives;

        public IReadOnlyList<UnmountedDriveModel> UnmountedDrives => _unmountedDrives;

        public event EventHandler<UnmountedDriveEventArgs> DriveAdded;

        public event EventHandler<UnmountedDriveEventArgs> DriveRemoved;

        protected UnmountedDriveServiceBase()
        {
            _unmountedDrives = new List<UnmountedDriveModel>();
        }

        public async Task ReloadUnmountedDrivesAsync()
        {
            var unmountedDrives = await GetUnmountedDrivesAsync();

            var oldRoots = _unmountedDrives.Select(d => d.FullName).ToHashSet();
            var newRoots = unmountedDrives.Select(d => d.FullName).ToHashSet();

            var addedDrives = unmountedDrives
                .Where(udm => !oldRoots.Contains(udm.FullName))
                .ToArray();
            var removedDrives = UnmountedDrives
                .Where(udm => !newRoots.Contains(udm.FullName))
                .ToArray();

            foreach (var unmountedDriveModel in addedDrives)
            {
                _unmountedDrives.Add(unmountedDriveModel);

                DriveAdded.Raise(this, CreateFrom(unmountedDriveModel));
            }

            foreach (var unmountedDriveModel in removedDrives)
            {
                _unmountedDrives.Remove(unmountedDriveModel);

                DriveRemoved.Raise(this, CreateFrom(unmountedDriveModel));
            }
        }

        public abstract void Mount(string drive);

        protected abstract Task<IReadOnlyList<UnmountedDriveModel>> GetUnmountedDrivesAsync();

        private static UnmountedDriveEventArgs CreateFrom(UnmountedDriveModel unmountedDriveModel) =>
            new UnmountedDriveEventArgs(unmountedDriveModel);
    }
}