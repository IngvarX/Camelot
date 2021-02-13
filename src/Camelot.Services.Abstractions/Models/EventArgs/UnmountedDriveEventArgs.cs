namespace Camelot.Services.Abstractions.Models.EventArgs
{
    public class UnmountedDriveEventArgs : System.EventArgs
    {
        public UnmountedDriveModel UnmountedDriveModel { get; }

        public UnmountedDriveEventArgs(UnmountedDriveModel unmountedDriveModel)
        {
            UnmountedDriveModel = unmountedDriveModel;
        }
    }
}