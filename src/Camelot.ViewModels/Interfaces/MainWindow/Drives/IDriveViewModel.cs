using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.Drives
{
    public interface IDriveViewModel
    {
        event EventHandler<EventArgs> OpeningRequested;

        string RootDirectory { get; }
    }
}