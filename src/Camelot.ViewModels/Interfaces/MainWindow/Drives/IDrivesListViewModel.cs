using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.Drives
{
    public interface IDrivesListViewModel
    {
        event EventHandler<DriveOpenedEventArgs> DriveOpened;
    }
}