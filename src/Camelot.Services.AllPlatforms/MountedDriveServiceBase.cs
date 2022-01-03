using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.Services.Environment.Interfaces;
using DriveInfo = Camelot.Services.Environment.Models.DriveInfo;

namespace Camelot.Services.AllPlatforms;

public abstract class MountedDriveServiceBase : IMountedDriveService
{
    private readonly IEnvironmentDriveService _environmentDriveService;

    private readonly List<DriveModel> _mountedDrives;

    public IReadOnlyList<DriveModel> MountedDrives => _mountedDrives;

    public event EventHandler<MountedDriveEventArgs> DriveAdded;

    public event EventHandler<MountedDriveEventArgs> DriveRemoved;

    public event EventHandler<MountedDriveEventArgs> DriveUpdated;

    protected MountedDriveServiceBase(
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
        var updatedDrives = _mountedDrives.Except(removedDrives).ToArray();

        foreach (var driveModel in updatedDrives)
        {
            var newDriveModel = drives.Single(d => d.RootDirectory == driveModel.RootDirectory);
            if (!CheckIfDiffer(driveModel, newDriveModel))
            {
                continue;
            }

            Update(driveModel, newDriveModel);

            DriveUpdated.Raise(this, CreateFrom(driveModel));
        }

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
    }

    public abstract void Unmount(string driveRootDirectory);

    public abstract Task EjectAsync(string driveRootDirectory);

    private IReadOnlyList<DriveModel> GetMountedDrives() =>
        _environmentDriveService
            .GetMountedDrives()
            .Where(Filter)
            .Select(CreateFrom)
            .ToArray();

    private static DriveModel CreateFrom(DriveInfo driveInfo) => new DriveModel
    {
        Name = driveInfo.Name,
        RootDirectory = driveInfo.RootDirectory,
        FreeSpaceBytes = driveInfo.FreeSpaceBytes,
        TotalSpaceBytes = driveInfo.TotalSpaceBytes
    };

    private static bool Filter(DriveInfo driveInfo)
    {
        try
        {
            return driveInfo.DriveType != DriveType.Ram
                   && driveInfo.DriveType != DriveType.Unknown
                   && driveInfo.TotalSpaceBytes > 0
                   && !driveInfo.RootDirectory.StartsWith("/snap/");
        }
        catch
        {
            return false;
        }
    }

    private static MountedDriveEventArgs CreateFrom(DriveModel driveModel) =>
        new(driveModel);

    private static bool CheckIfDiffer(DriveModel oldDriveModel, DriveModel newDriveModel) =>
        oldDriveModel.Name != newDriveModel.Name
        || oldDriveModel.FreeSpaceBytes != newDriveModel.FreeSpaceBytes
        || oldDriveModel.TotalSpaceBytes != newDriveModel.TotalSpaceBytes;

    private static void Update(DriveModel driveModel, DriveModel newDriveModel)
    {
        driveModel.Name = newDriveModel.Name;
        driveModel.FreeSpaceBytes = newDriveModel.FreeSpaceBytes;
        driveModel.TotalSpaceBytes = newDriveModel.TotalSpaceBytes;
    }
}