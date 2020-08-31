using System.IO;

namespace Camelot.Services.Environment.Interfaces
{
    public interface IEnvironmentDriveService
    {
        DriveInfo[] GetMountedDrives();
    }
}