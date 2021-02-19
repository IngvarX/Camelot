using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
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
        private readonly IMountedDriveService _mountedDriveService;

        private readonly string _rootDirectory;

        private string _name;
        private long _freeSpaceBytes;
        private long _totalSpaceBytes;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                this.RaisePropertyChanged(nameof(DriveName));
            }
        }

        public long FreeSpaceBytes
        {
            get => _freeSpaceBytes;
            set
            {
                _freeSpaceBytes = value;
                this.RaisePropertyChanged(nameof(AvailableSizeAsNumber));
                this.RaisePropertyChanged(nameof(AvailableFormattedSize));
            }
        }

        public long TotalSpaceBytes
        {
            get => _totalSpaceBytes;
            set
            {
                _totalSpaceBytes = value;
                this.RaisePropertyChanged(nameof(TotalSizeAsNumber));
                this.RaisePropertyChanged(nameof(TotalFormattedSize));
            }
        }

        public string DriveName => _pathService.GetFileName(Name);

        public string AvailableSizeAsNumber => _fileSizeFormatter.GetSizeAsNumber(FreeSpaceBytes);

        public string AvailableFormattedSize => _fileSizeFormatter.GetFormattedSize(FreeSpaceBytes);

        public string TotalSizeAsNumber => _fileSizeFormatter.GetSizeAsNumber(TotalSpaceBytes);

        public string TotalFormattedSize => _fileSizeFormatter.GetFormattedSize(TotalSpaceBytes);

        public ICommand OpenCommand { get; }

        public ICommand UnmountCommand { get; }

        public DriveViewModel(
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService,
            IFilesOperationsMediator filesOperationsMediator,
            IMountedDriveService mountedDriveService,
            DriveModel driveModel)
        {
            _fileSizeFormatter = fileSizeFormatter;
            _pathService = pathService;
            _filesOperationsMediator = filesOperationsMediator;
            _mountedDriveService = mountedDriveService;

            _rootDirectory = driveModel.RootDirectory;

            Name = driveModel.Name;
            FreeSpaceBytes = driveModel.FreeSpaceBytes;
            TotalSpaceBytes = driveModel.TotalSpaceBytes;

            OpenCommand = ReactiveCommand.Create(Open);
            UnmountCommand = ReactiveCommand.Create(Unmount);
        }

        private void Open() =>
            _filesOperationsMediator.ActiveFilesPanelViewModel.CurrentDirectory = _rootDirectory;


        private void Unmount() => _mountedDriveService.Unmount(_rootDirectory);
    }
}