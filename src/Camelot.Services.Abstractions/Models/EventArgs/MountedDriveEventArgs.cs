namespace Camelot.Services.Abstractions.Models.EventArgs
{
    public class MountedDriveEventArgs : System.EventArgs
    {
        public DriveModel DriveModel { get; }

        public MountedDriveEventArgs(DriveModel driveModel)
        {
            DriveModel = driveModel;
        }
    }
}