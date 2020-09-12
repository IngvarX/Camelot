using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;

namespace Camelot.ViewModels.Implementations.MainWindow.Drives
{
    public class UnmountedDriveViewModel : ViewModelBase, IDriveViewModel
    {
        private readonly UnmountedDriveModel _unmountedDriveModel;

        public string DriveName => _unmountedDriveModel.Name;

        public UnmountedDriveViewModel(UnmountedDriveModel unmountedDriveModel)
        {
            _unmountedDriveModel = unmountedDriveModel;
        }
    }
}