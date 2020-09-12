using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using DynamicData;

namespace Camelot.ViewModels.Implementations.MainWindow.Drives
{
    public class DrivesListViewModel : ViewModelBase, IDrivesListViewModel
    {
        private readonly IDriveService _driveService;
        private readonly IDriveViewModelFactory _driveViewModelFactory;
        private readonly IApplicationDispatcher _applicationDispatcher;
        private readonly ObservableCollection<IDriveViewModel> _drives;

        public IEnumerable<IDriveViewModel> Drives => _drives;

        public DrivesListViewModel(
            IDriveService driveService,
            IDriveViewModelFactory driveViewModelFactory,
            IApplicationDispatcher applicationDispatcher)
        {
            _driveService = driveService;
            _driveViewModelFactory = driveViewModelFactory;
            _applicationDispatcher = applicationDispatcher;
            _drives = new ObservableCollection<IDriveViewModel>();

            SubscribeToEvents();
            ReloadDrives();
        }

        private void DriveServiceOnDrivesListChanged(object sender, EventArgs args) =>
            ReloadDrives();

        private void ReloadDrives()
        {
            var mountedDrives = _driveService
                .MountedDrives
                .Select(_driveViewModelFactory.Create);
            var unmountedDrives = _driveService
                .UnmountedDrives
                .Select(_driveViewModelFactory.Create);
            var drives = mountedDrives.Concat(unmountedDrives).ToArray();

            _applicationDispatcher.Dispatch(() =>
            {
                _drives.Clear();
                _drives.AddRange(drives);
            });
        }

        private void SubscribeToEvents() => _driveService.DrivesListChanged += DriveServiceOnDrivesListChanged;
    }
}