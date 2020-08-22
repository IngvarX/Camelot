using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions
{
    public interface IDriveService
    {
        IReadOnlyList<DriveModel> Drives { get; }

        event EventHandler<DrivesListChangedEventArgs> DrivesListChanged;

        DriveModel GetFileDrive(string filePath);
    }
}