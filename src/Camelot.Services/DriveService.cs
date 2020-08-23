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
        private readonly Timer _timer;

        public IReadOnlyList<DriveModel> Drives { get; private set; }

        public event EventHandler<EventArgs> DrivesListChanged;

        public DriveService(
            IEnvironmentDriveService environmentDriveService,
            DriveServiceConfiguration configuration)
        {
            _environmentDriveService = environmentDriveService;
            _timer = new Timer(configuration.DrivesListRefreshIntervalMs);

            Drives = GetDrives();
            SetupTimer();
        }

        public DriveModel GetFileDrive(string filePath) =>
            GetDrives()
                .OrderByDescending(d => d.RootDirectory.Length)
                .First(d => filePath.StartsWith(d.RootDirectory));

        private void SetupTimer()
        {
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            var oldRoots = Drives.Select(d => d.RootDirectory).ToHashSet();

            var drives = GetDrives();
            var newRoots = drives.Select(d => d.RootDirectory).ToHashSet();

            var addedDrives = drives.Where(dm => !oldRoots.Contains(dm.RootDirectory));
            var removedDrives = Drives.Where(dm => !newRoots.Contains(dm.RootDirectory));

            if (addedDrives.Any() || removedDrives.Any())
            {
                Drives = drives;

                DrivesListChanged.Raise(this, EventArgs.Empty);
            }
        }

        private IReadOnlyList<DriveModel> GetDrives() =>
            _environmentDriveService
                .GetDrives()
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
    }
}