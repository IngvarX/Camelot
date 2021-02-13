using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Avalonia.Interfaces;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.Drives
{
    public class DrivesListViewModel : ViewModelBase, IDrivesListViewModel
    {
        private readonly IMountedDriveService _mountedDriveService;
        private readonly IUnmountedDriveService _unmountedDriveService;
        private readonly IDriveViewModelFactory _driveViewModelFactory;
        private readonly IApplicationDispatcher _applicationDispatcher;

        private readonly Dictionary<DriveModel, IDriveViewModel> _mountedDrivesDictionary;
        private readonly Dictionary<UnmountedDriveModel, IDriveViewModel> _unmountedDrivesDictionary;

        public IEnumerable<IDriveViewModel> Drives =>
            _mountedDrivesDictionary.Values.Concat(_unmountedDrivesDictionary.Values);

        public DrivesListViewModel(
            IMountedDriveService mountedDriveService,
            IUnmountedDriveService unmountedDriveService,
            IDrivesUpdateService drivesUpdateService,
            IDriveViewModelFactory driveViewModelFactory,
            IApplicationDispatcher applicationDispatcher)
        {
            _mountedDriveService = mountedDriveService;
            _unmountedDriveService = unmountedDriveService;
            _driveViewModelFactory = driveViewModelFactory;
            _applicationDispatcher = applicationDispatcher;

            _mountedDrivesDictionary = new Dictionary<DriveModel, IDriveViewModel>();
            _unmountedDrivesDictionary = new Dictionary<UnmountedDriveModel, IDriveViewModel>();

            SubscribeToEvents();
            LoadDrives();

            drivesUpdateService.Start();
        }

        private void LoadDrives()
        {
            _mountedDriveService.MountedDrives.ForEach(AddDrive);
            _unmountedDriveService.UnmountedDrives.ForEach(AddDrive);
        }

        private void SubscribeToEvents()
        {
            SubscribeToMountedDriveServiceEvents();
            SubscribeToUnmountedDriveServiceEvents();
        }

        private void SubscribeToMountedDriveServiceEvents()
        {
            _mountedDriveService.DriveAdded += MountedDriveServiceOnDriveAdded;
            _mountedDriveService.DriveRemoved += MountedDriveServiceOnDriveRemoved;
            _mountedDriveService.DriveUpdated += MountedDriveServiceOnDriveUpdated;
        }

        private void SubscribeToUnmountedDriveServiceEvents()
        {
            _unmountedDriveService.DriveAdded += UnmountedDriveServiceOnDriveAdded;
            _unmountedDriveService.DriveRemoved += UnmountedDriveServiceOnDriveRemoved;
        }

        private void MountedDriveServiceOnDriveAdded(object sender, MountedDriveEventArgs e) =>
            AddDrive(e.DriveModel);

        private void MountedDriveServiceOnDriveRemoved(object sender, MountedDriveEventArgs e) =>
            RemoveDrive(e.DriveModel);

        private void MountedDriveServiceOnDriveUpdated(object sender, MountedDriveEventArgs e) =>
            UpdateDrive(e.DriveModel);

        private void UnmountedDriveServiceOnDriveAdded(object sender, UnmountedDriveEventArgs e) =>
            AddDrive(e.UnmountedDriveModel);

        private void UnmountedDriveServiceOnDriveRemoved(object sender, UnmountedDriveEventArgs e) =>
            RemoveDrive(e.UnmountedDriveModel);

        private void AddDrive(DriveModel driveModel)
        {
            var driveViewModel = CreateFrom(driveModel);
            _mountedDrivesDictionary[driveModel] = driveViewModel;

            UpdateDrivesList();
        }

        private void RemoveDrive(DriveModel driveModel)
        {
            _mountedDrivesDictionary.Remove(driveModel);

            UpdateDrivesList();
        }

        private void UpdateDrive(DriveModel driveModel)
        {
            var viewModel = _mountedDrivesDictionary[driveModel] as IMountedDriveViewModel;
            // TODO: update
        }

        private void AddDrive(UnmountedDriveModel unmountedDriveModel)
        {
            var driveViewModel = CreateFrom(unmountedDriveModel);
            _unmountedDrivesDictionary[unmountedDriveModel] = driveViewModel;

            UpdateDrivesList();
        }

        private void RemoveDrive(UnmountedDriveModel unmountedDriveModel)
        {
            _unmountedDrivesDictionary.Remove(unmountedDriveModel);

            UpdateDrivesList();
        }

        private IDriveViewModel CreateFrom(DriveModel driveModel) =>
            _driveViewModelFactory.Create(driveModel);

        private IDriveViewModel CreateFrom(UnmountedDriveModel unmountedDriveModel) =>
            _driveViewModelFactory.Create(unmountedDriveModel);

        private void UpdateDrivesList() =>
            _applicationDispatcher.Dispatch(() => this.RaisePropertyChanged(nameof(Drives)));
    }
}