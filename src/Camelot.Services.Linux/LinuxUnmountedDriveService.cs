using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Configuration;

namespace Camelot.Services.Linux
{
    public class LinuxUnmountedDriveService : UnmountedDriveServiceBase
    {
        private const string FindDrivesCommand = "lsblk";
        private const string FindDriveArguments = "--noheadings --raw -o NAME,MOUNTPOINT";
        private const string MountDriveCommand = "udisksctl";
        private const string MountDriveArguments = "mount -b {0}";

        private readonly IProcessService _processService;
        private readonly IEnvironmentService _environmentService;
        private readonly UnmountedDrivesConfiguration _configuration;

        public LinuxUnmountedDriveService(
            IProcessService processService,
            IEnvironmentService environmentService,
            UnmountedDrivesConfiguration configuration)
        {
            _processService = processService;
            _environmentService = environmentService;
            _configuration = configuration;
        }

        public override void Mount(string drive)
        {
            var arguments = string.Format(MountDriveArguments, drive);

            _processService.Run(MountDriveCommand, arguments);
        }

        protected override async Task<IReadOnlyList<UnmountedDriveModel>> GetUnmountedDrivesAsync()
        {
            if (!_configuration.IsEnabled)
            {
                return Array.Empty<UnmountedDriveModel>();
            }

            try
            {
                return await GetUnmountedDrivesUsingLsblkAsync();
            }
            catch
            {
                return Array.Empty<UnmountedDriveModel>();
            }
        }

        private async Task<IReadOnlyList<UnmountedDriveModel>> GetUnmountedDrivesUsingLsblkAsync()
        {
            var drives = await _processService.ExecuteAndGetOutputAsync(FindDrivesCommand, FindDriveArguments);

            return drives
                .Split(_environmentService.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split())
                .Where(Filter)
                .Select(CreateFrom)
                .ToArray();
        }

        private static bool Filter(string[] driveData)
        {
            if (driveData.Length < 2)
            {
                return false;
            }

            var driveMountPoint = driveData[1];
            if (!string.IsNullOrWhiteSpace(driveMountPoint))
            {
                return false;
            }

            var driveName = driveData[0];

            return !string.IsNullOrWhiteSpace(driveName)
                   && !driveName.StartsWith("loop")
                   && char.IsDigit(driveName[^1]);
        }

        private static UnmountedDriveModel CreateFrom(string[] driveData) =>
            new UnmountedDriveModel
            {
                Name = driveData[0],
                FullName = GetFullName(driveData[0])
            };

        private static string GetFullName(string shortName) => $"/dev/{shortName}";
    }
}