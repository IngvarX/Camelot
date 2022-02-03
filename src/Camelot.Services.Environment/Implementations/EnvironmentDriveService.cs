using System.Collections.Generic;
using System.Linq;
using Camelot.Extensions;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Environment.Models;
using IoDriveInfo = System.IO.DriveInfo;

namespace Camelot.Services.Environment.Implementations;

public class EnvironmentDriveService : IEnvironmentDriveService
{
    public IReadOnlyList<DriveInfo> GetMountedDrives() =>
        IoDriveInfo
            .GetDrives()
            .Select(CreateFrom)
            .WhereNotNull()
            .ToArray();

    private static DriveInfo CreateFrom(IoDriveInfo ioDriveInfo)
    {
        try
        {
            return new DriveInfo
            {
                RootDirectory = ioDriveInfo.RootDirectory.FullName,
                Name = ioDriveInfo.Name,
                TotalSpaceBytes = ioDriveInfo.TotalSize,
                FreeSpaceBytes = ioDriveInfo.AvailableFreeSpace,
                DriveType = ioDriveInfo.DriveType,
                DriveFormat = ioDriveInfo.DriveFormat
            };
        }
        catch
        {
            return null;
        }
    }
}