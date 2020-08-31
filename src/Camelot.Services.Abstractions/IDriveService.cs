using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface IDriveService
    {
        IReadOnlyList<DriveModel> MountedDrives { get; }

        IReadOnlyList<UnmountedDriveModel> UnmountedDrives { get; }

        event EventHandler<EventArgs> DrivesListChanged;

        DriveModel GetFileDrive(string filePath);
    }
}