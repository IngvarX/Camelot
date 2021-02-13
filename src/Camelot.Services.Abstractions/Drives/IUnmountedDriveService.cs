using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions.Drives
{
    public interface IUnmountedDriveService
    {
        IReadOnlyList<UnmountedDriveModel> UnmountedDrives { get; }

        event EventHandler<UnmountedDriveEventArgs> DriveAdded;

        event EventHandler<UnmountedDriveEventArgs> DriveRemoved;

        Task ReloadUnmountedDrivesAsync();

        void Mount(string drive);
    }
}