using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.Drives
{
    public class DriveViewModel : ViewModelBase, IDriveViewModel, IMountedDriveViewModel
    {
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IPathService _pathService;
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly DriveModel _driveModel;

        public string DriveName => _pathService.GetFileName(_driveModel.Name);

        public string AvailableSizeAsNumber => _fileSizeFormatter.GetSizeAsNumber(_driveModel.FreeSpaceBytes);

        public string AvailableFormattedSize => _fileSizeFormatter.GetFormattedSize(_driveModel.FreeSpaceBytes);

        public string TotalSizeAsNumber => _fileSizeFormatter.GetSizeAsNumber(_driveModel.TotalSpaceBytes);

        public string TotalFormattedSize => _fileSizeFormatter.GetFormattedSize(_driveModel.TotalSpaceBytes);

        public ICommand OpenCommand { get; }

        public DriveViewModel(
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService,
            IFilesOperationsMediator filesOperationsMediator,
            DriveModel driveModel)
        {
            _fileSizeFormatter = fileSizeFormatter;
            _pathService = pathService;
            _filesOperationsMediator = filesOperationsMediator;
            _driveModel = driveModel;

            OpenCommand = ReactiveCommand.Create(Open);
        }

        private void Open() =>
            _filesOperationsMediator.ActiveFilesPanelViewModel.CurrentDirectory = _driveModel.RootDirectory;
    }
}