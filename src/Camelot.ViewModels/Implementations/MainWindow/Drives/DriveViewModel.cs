using System.Windows.Input;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.Drives
{
    public class DriveViewModel : ViewModelBase, IDriveViewModel
    {
        private readonly DriveModel _driveModel;

        public string DriveName => _driveModel.Name;

        public string RootDirectory => _driveModel.RootDirectory;

        [Reactive]
        public bool IsSelected { get; set; }

        public ICommand SelectCommand { get; }

        public DriveViewModel(
            DriveModel driveModel)
        {
            _driveModel = driveModel;

            SelectCommand = ReactiveCommand.Create(Select);
        }

        private void Select()
        {

        }
    }
}