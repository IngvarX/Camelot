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
        private readonly IPathService _pathService;

        public DriveViewModelFactory(
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService)
        {
            _fileSizeFormatter = fileSizeFormatter;
            _pathService = pathService;
        }

        public IDriveViewModel Create(DriveModel driveModel) =>
            new DriveViewModel(_fileSizeFormatter, _pathService, driveModel);
    }
}