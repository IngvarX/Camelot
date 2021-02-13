using System.Windows.Input;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.Drives
{
    public class UnmountedDriveViewModel : ViewModelBase, IDriveViewModel
    {
        private readonly IUnmountedDriveService _unmountedDriveService;
        private readonly UnmountedDriveModel _unmountedDriveModel;

        public string DriveName => _unmountedDriveModel.Name;

        public ICommand MountCommand { get; }

        public UnmountedDriveViewModel(
            IUnmountedDriveService unmountedDriveService,
            UnmountedDriveModel unmountedDriveModel)
        {
            _unmountedDriveService = unmountedDriveService;
            _unmountedDriveModel = unmountedDriveModel;

            MountCommand = ReactiveCommand.Create(Mount);
        }

        private void Mount() => _unmountedDriveService.Mount(_unmountedDriveModel.FullName);
    }
}