using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.Drives
{
    public class DriveOpenedEventArgs : EventArgs
    {
        public IDriveViewModel Drive { get; }

        public DriveOpenedEventArgs(IDriveViewModel drive)
        {
            Drive = drive;
        }
    }
}