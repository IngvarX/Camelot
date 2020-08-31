using System.IO;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Environment.Implementations
{
    public class EnvironmentDriveService : IEnvironmentDriveService
    {
        public DriveInfo[] GetMountedDrives() => DriveInfo.GetDrives();
    }
}