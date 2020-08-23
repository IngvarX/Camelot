using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Extensions;
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
        private readonly ObservableCollection<IDriveViewModel> _drives;

        public IEnumerable<IDriveViewModel> Drives => _drives;

        public event EventHandler<DriveOpenedEventArgs> DriveOpened;

        public DrivesListViewModel(
            IDriveService driveService,
            IDriveViewModelFactory driveViewModelFactory)
        {
            _driveService = driveService;
            _driveViewModelFactory = driveViewModelFactory;
            _drives = new ObservableCollection<IDriveViewModel>();

            SubscribeToEvents();
            ReloadDrives();
        }

        private void DriveServiceOnDrivesListChanged(object sender, EventArgs args) =>
            ReloadDrives();

        private void ReloadDrives()
        {
            var drives = _driveService
                .Drives
                .Select(_driveViewModelFactory.Create)
                .ToArray();
            drives.ForEach(SubscribeToEvents);

            _drives.ForEach(UnsubscribeFromEvents);
            _drives.Clear();
            _drives.AddRange(drives);
        }

        private void SubscribeToEvents() => _driveService.DrivesListChanged += DriveServiceOnDrivesListChanged;

        private void SubscribeToEvents(IDriveViewModel driveViewModel) =>
            driveViewModel.OpeningRequested += DriveViewModelOnOpeningRequested;

        private void UnsubscribeFromEvents(IDriveViewModel driveViewModel) =>
            driveViewModel.OpeningRequested -= DriveViewModelOnOpeningRequested;

        private void DriveViewModelOnOpeningRequested(object sender, EventArgs e)
        {
            var viewModel = (IDriveViewModel) sender;
            var args = new DriveOpenedEventArgs(viewModel);

            DriveOpened.Raise(this, args);
        }
    }
}