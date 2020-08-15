using System.Collections.Generic;
using System.IO;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services
{
    public class DriveService : IDriveService
    {
        private readonly IEnvironmentDriveService _environmentDriveService;

        public DriveService(IEnvironmentDriveService environmentDriveService)
        {
            _environmentDriveService = environmentDriveService;
        }

        public IReadOnlyList<DriveModel> GetDrives() =>
            _environmentDriveService
                .GetDrives()
                .Where(d => d.DriveType != DriveType.Ram)
                .Select(CreateFrom)
                .ToArray();

        public DriveModel GetFileDrive(string filePath) =>
            GetDrives()
                .OrderByDescending(d => d.RootDirectory.Length)
                .First(d => filePath.StartsWith(d.RootDirectory));

        private static DriveModel CreateFrom(DriveInfo driveInfo) =>
            new DriveModel
            {
                Name = driveInfo.Name,
                RootDirectory = driveInfo.RootDirectory.FullName
            };
    }
}