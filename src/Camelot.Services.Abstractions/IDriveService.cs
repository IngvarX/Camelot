using System.Collections.Generic;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface IDriveService
    {
        IReadOnlyList<DriveModel> GetDrives();

        DriveModel GetFileDrive(string filePath);
    }
}