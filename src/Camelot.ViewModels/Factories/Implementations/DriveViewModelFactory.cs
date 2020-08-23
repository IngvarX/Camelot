using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class DriveViewModelFactory : IDriveViewModelFactory
    {
        private readonly IFileSizeFormatter _fileSizeFormatter;

        public DriveViewModelFactory(IFileSizeFormatter fileSizeFormatter)
        {
            _fileSizeFormatter = fileSizeFormatter;
        }

        public IDriveViewModel Create(DriveModel driveModel) =>
            new DriveViewModel(_fileSizeFormatter, driveModel);
    }
}