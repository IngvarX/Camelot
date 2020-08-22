namespace Camelot.ViewModels.Interfaces.MainWindow.Drives
{
    public interface IDriveViewModel
    {
        string RootDirectory { get; }

        bool IsSelected { get; set; }
    }
}