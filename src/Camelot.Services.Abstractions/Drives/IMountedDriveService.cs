using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Drives;

public interface IMountedDriveService
{
    IReadOnlyList<DriveModel> MountedDrives { get; }

    event EventHandler<MountedDriveEventArgs> DriveAdded;

    event EventHandler<MountedDriveEventArgs> DriveRemoved;

    event EventHandler<MountedDriveEventArgs> DriveUpdated;

    DriveModel GetFileDrive(string filePath);

    void ReloadMountedDrives();

    void Unmount(string driveRootDirectory);

    Task EjectAsync(string driveRootDirectory);
}