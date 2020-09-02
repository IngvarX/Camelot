using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Configuration;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services
{
    public class DriveService : IDriveService
    {
        private readonly IEnvironmentDriveService _environmentDriveService;
        private readonly IUnmountedDriveService _unmountedDriveService;
        private readonly Timer _timer;

        private readonly List<DriveModel> _mountedDrives;
        private readonly List<UnmountedDriveModel> _unmountedDrives;

        public IReadOnlyList<DriveModel> MountedDrives => _mountedDrives;

        public IReadOnlyList<UnmountedDriveModel> UnmountedDrives => _unmountedDrives;

        public event EventHandler<EventArgs> DrivesListChanged;

        public DriveService(
            IEnvironmentDriveService environmentDriveService,
            IUnmountedDriveService unmountedDriveService,
            DriveServiceConfiguration configuration)
        {
            _environmentDriveService = environmentDriveService;
            _unmountedDriveService = unmountedDriveService;

            _mountedDrives = new List<DriveModel>();
            _unmountedDrives = new List<UnmountedDriveModel>();
            _timer = new Timer(configuration.DrivesListRefreshIntervalMs);

            ReloadDrives();
            SetupTimer();
        }

        public DriveModel GetFileDrive(string filePath) =>
            GetMountedDrives()
                .OrderByDescending(d => d.RootDirectory.Length)
                .First(d => filePath.StartsWith(d.RootDirectory));

        private void SetupTimer()
        {
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e) => ReloadDrives();

        private void ReloadDrives()
        {
            ReloadMountedDrives();
            ReloadUnmountedDrives();
        }

        private void ReloadMountedDrives()
        {
            var oldRoots = _mountedDrives.Select(d => d.RootDirectory).ToHashSet();

            var drives = GetMountedDrives();
            var newRoots = drives.Select(d => d.RootDirectory).ToHashSet();

            var addedDrives = drives.Where(dm => !oldRoots.Contains(dm.RootDirectory));
            var removedDrives = MountedDrives.Where(dm => !newRoots.Contains(dm.RootDirectory));

            if (addedDrives.Any() || removedDrives.Any())
            {
                _mountedDrives.Clear();
                _mountedDrives.AddRange(drives);

                DrivesListChanged.Raise(this, EventArgs.Empty);
            }
        }

        private void ReloadUnmountedDrives()
        {
            var unmountedDrives = GetUnmountedDrives();
            _unmountedDrives.Clear();
            _unmountedDrives.AddRange(unmountedDrives);
        }

        private IReadOnlyList<DriveModel> GetMountedDrives() =>
            _environmentDriveService
                .GetMountedDrives()
                .Where(Filter)
                .Select(CreateFrom)
                .WhereNotNull()
                .ToArray();

        private IReadOnlyList<UnmountedDriveModel> GetUnmountedDrives() =>
            _unmountedDriveService
                .GetUnmountedDrives()
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
                    TotalSpaceBytes = driveInfo.TotalSize,
                    IsMounted = true
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
    }
}