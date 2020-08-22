using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class DriveViewModelFactory : IDriveViewModelFactory
    {
        public IDriveViewModel Create(DriveModel driveModel) => new DriveViewModel(driveModel);
    }
}