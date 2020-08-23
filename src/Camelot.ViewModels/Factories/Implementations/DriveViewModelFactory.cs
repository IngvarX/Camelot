using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class DriveViewModelFactory : IDriveViewModelFactory
    {
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IPathService _pathService;
        private readonly IFilesOperationsMediator _filesOperationsMediator;

        public DriveViewModelFactory(
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService,
            IFilesOperationsMediator filesOperationsMediator)
        {
            _fileSizeFormatter = fileSizeFormatter;
            _pathService = pathService;
            _filesOperationsMediator = filesOperationsMediator;
        }

        public IDriveViewModel Create(DriveModel driveModel) =>
            new DriveViewModel(_fileSizeFormatter, _pathService, _filesOperationsMediator, driveModel);
    }
}