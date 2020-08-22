using System.Collections.Generic;

namespace Camelot.Services.Abstractions.Models.EventArgs
{
    public class DrivesListChangedEventArgs : System.EventArgs
    {
        public IReadOnlyList<DriveModel> AddedDrives { get; }

        public IReadOnlyList<DriveModel> RemovedDrives { get; }

        public DrivesListChangedEventArgs(
            IReadOnlyList<DriveModel> addedDrives,
            IReadOnlyList<DriveModel> removedDrives)
        {
            AddedDrives = addedDrives;
            RemovedDrives = removedDrives;
        }
    }
}