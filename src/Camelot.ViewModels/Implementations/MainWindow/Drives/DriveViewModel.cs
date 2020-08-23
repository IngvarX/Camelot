using System;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.Drives
{
    public class DriveViewModel : ViewModelBase, IDriveViewModel
    {
        private readonly DriveModel _driveModel;

        public string DriveName => _driveModel.Name;

        public string RootDirectory => _driveModel.RootDirectory;

        public event EventHandler<EventArgs> OpeningRequested;

        public ICommand OpenCommand { get; }

        public DriveViewModel(DriveModel driveModel)
        {
            _driveModel = driveModel;

            OpenCommand = ReactiveCommand.Create(Open);
        }

        private void Open() => OpeningRequested.Raise(this, EventArgs.Empty);
    }
}