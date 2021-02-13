using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Drives
{
    public class MountedDriveService : IMountedDriveService
    {
        private readonly IEnvironmentDriveService _environmentDriveService;

        private readonly List<DriveModel> _mountedDrives;

        public IReadOnlyList<DriveModel> MountedDrives => _mountedDrives;

        public event EventHandler<MountedDriveEventArgs> DriveAdded;

        public event EventHandler<MountedDriveEventArgs> DriveRemoved;

        public event EventHandler<MountedDriveEventArgs> DriveUpdated;

        public MountedDriveService(
            IEnvironmentDriveService environmentDriveService)
        {
            _environmentDriveService = environmentDriveService;

            _mountedDrives = new List<DriveModel>();

            ReloadMountedDrives();
        }

        public DriveModel GetFileDrive(string filePath) =>
            MountedDrives
                .OrderByDescending(d => d.RootDirectory.Length)
                .First(d => filePath.StartsWith(d.RootDirectory));

        public void ReloadMountedDrives()
        {
            var oldRoots = _mountedDrives.Select(d => d.RootDirectory).ToHashSet();

            var drives = GetMountedDrives();
            var newRoots = drives.Select(d => d.RootDirectory).ToHashSet();

            var addedDrives = drives
                .Where(dm => !oldRoots.Contains(dm.RootDirectory))
                .ToArray();
            var removedDrives = MountedDrives
                .Where(dm => !newRoots.Contains(dm.RootDirectory))
                .ToArray();

            foreach (var driveModel in addedDrives)
            {
                _mountedDrives.Add(driveModel);

                DriveAdded.Raise(this, CreateFrom(driveModel));
            }

            foreach (var driveModel in removedDrives)
            {
                _mountedDrives.Remove(driveModel);

                DriveRemoved.Raise(this, CreateFrom(driveModel));
            }

            // TODO: updated drives
        }

        private IReadOnlyList<DriveModel> GetMountedDrives() =>
            _environmentDriveService
                .GetMountedDrives()
                .Where(Filter)
                .Select(CreateFrom)
                .WhereNotNull()
                .ToArray();

        private static DriveModel CreateFrom(DriveInfo driveInfo)
        {
            try
            {
                return new DriveModel
                {
                    Name = driveInfo.Name,
                    RootDirectory = driveInfo.RootDirectory.FullName,
                    FreeSpaceBytes = driveInfo.AvailableFreeSpace,
                    TotalSpaceBytes = driveInfo.TotalSize
                };
            }
            catch
            {
                return null;
            }
        }

        private static bool Filter(DriveInfo driveInfo)
        {
            try
            {
                return driveInfo.DriveType != DriveType.Ram
                       && driveInfo.DriveType != DriveType.Unknown
                       && driveInfo.DriveFormat != "fuse"
                       && !driveInfo.RootDirectory.FullName.StartsWith("/snap/")
                       && !driveInfo.RootDirectory.FullName.StartsWith("/sys/");
            }
            catch
            {
                return false;
            }
        }

        private static MountedDriveEventArgs CreateFrom(DriveModel driveModel) =>
            new MountedDriveEventArgs(driveModel);
    }
}