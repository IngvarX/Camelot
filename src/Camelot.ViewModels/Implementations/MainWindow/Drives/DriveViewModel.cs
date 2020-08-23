using System;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.Drives
{
    public class DriveViewModel : ViewModelBase, IDriveViewModel
    {
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly DriveModel _driveModel;

        public string DriveName => _driveModel.Name;

        public string RootDirectory => _driveModel.RootDirectory;

        public string AvailableSizeAsNumber => _fileSizeFormatter.GetSizeAsNumber(_driveModel.FreeSpaceBytes);

        public string AvailableFormattedSize => _fileSizeFormatter.GetFormattedSize(_driveModel.FreeSpaceBytes);

        public string TotalSizeAsNumber => _fileSizeFormatter.GetSizeAsNumber(_driveModel.TotalSpaceBytes);

        public string TotalFormattedSize => _fileSizeFormatter.GetFormattedSize(_driveModel.TotalSpaceBytes);

        public event EventHandler<EventArgs> OpeningRequested;

        public ICommand OpenCommand { get; }

        public DriveViewModel(
            IFileSizeFormatter fileSizeFormatter,
            DriveModel driveModel)
        {
            _fileSizeFormatter = fileSizeFormatter;
            _driveModel = driveModel;

            OpenCommand = ReactiveCommand.Create(Open);
        }

        private void Open() => OpeningRequested.Raise(this, EventArgs.Empty);
    }
}