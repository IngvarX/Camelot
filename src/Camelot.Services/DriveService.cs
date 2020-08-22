using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Configuration;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services
{
    public class DriveService : IDriveService
    {
        private readonly IEnvironmentDriveService _environmentDriveService;
        private readonly Timer _timer;

        public IReadOnlyList<DriveModel> Drives { get; private set; }

        public event EventHandler<DrivesListChangedEventArgs> DrivesListChanged;

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

            var addedDrives = drives.Where(dm => !oldRoots.Contains(dm.RootDirectory)).ToArray();
            var removedDrives = Drives.Where(dm => !newRoots.Contains(dm.RootDirectory)).ToArray();

            Drives = drives;

            var args = new DrivesListChangedEventArgs(addedDrives, removedDrives);
            DrivesListChanged.Raise(this, args);
        }

        private IReadOnlyList<DriveModel> GetDrives() =>
            _environmentDriveService
                .GetDrives()
                .Where(d => d.DriveType != DriveType.Ram)
                .Select(CreateFrom)
                .ToArray();

        private static DriveModel CreateFrom(DriveInfo driveInfo) =>
            new DriveModel
            {
                Name = driveInfo.Name,
                RootDirectory = driveInfo.RootDirectory.FullName
            };
    }
}