namespace Camelot.ViewModels.Interfaces.MainWindow.Drives;

public interface IMountedDriveViewModel
{
    string Name { get; set; }

    long FreeSpaceBytes { get; set; }

    long TotalSpaceBytes { get; set; }
}