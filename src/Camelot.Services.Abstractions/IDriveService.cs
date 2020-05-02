using System.Collections.Generic;
using Camelot.Services.Models;

namespace Camelot.Services.Abstractions
{
    public interface IDriveService
    {
        IReadOnlyCollection<DriveModel> GetDrives();

        DriveModel GetFileDrive(string filePath);
    }
}