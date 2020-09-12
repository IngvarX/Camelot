using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IDriveViewModelFactory
    {
        IDriveViewModel Create(DriveModel driveModel);

        IDriveViewModel Create(UnmountedDriveModel unmountedDriveModel);
    }
}