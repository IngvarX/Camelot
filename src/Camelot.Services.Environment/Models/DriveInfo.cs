using DriveType =  System.IO.DriveType;
using IoDriveInfo = System.IO.DriveInfo;

namespace Camelot.Services.Environment.Models;

public class DriveInfo
{
    public string RootDirectory { get; set; }

    public string Name { get; set; }

    public long TotalSpaceBytes { get; set; }

    public long FreeSpaceBytes { get; set; }

    public DriveType DriveType { get; set; }

    public string DriveFormat { get; set; }
}