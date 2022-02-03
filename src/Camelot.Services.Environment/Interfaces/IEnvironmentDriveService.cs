using System.Collections.Generic;
using Camelot.Services.Environment.Models;

namespace Camelot.Services.Environment.Interfaces;

public interface IEnvironmentDriveService
{
    IReadOnlyList<DriveInfo> GetMountedDrives();
}