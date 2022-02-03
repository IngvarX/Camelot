using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Environment.Interfaces;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.Drives;
using Camelot.ViewModels.Interfaces.MainWindow.Drives;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations;

public class DriveViewModelFactory : IDriveViewModelFactory
{
    private readonly IFileSizeFormatter _fileSizeFormatter;
    private readonly IPathService _pathService;
    private readonly IFilesOperationsMediator _filesOperationsMediator;
    private readonly IUnmountedDriveService _unmountedDriveService;
    private readonly IMountedDriveService _mountedDriveService;
    private readonly IPlatformService _platformService;

    public DriveViewModelFactory(
        IFileSizeFormatter fileSizeFormatter,
        IPathService pathService,
        IFilesOperationsMediator filesOperationsMediator,
        IUnmountedDriveService unmountedDriveService,
        IMountedDriveService mountedDriveService,
        IPlatformService platformService)
    {
        _fileSizeFormatter = fileSizeFormatter;
        _pathService = pathService;
        _filesOperationsMediator = filesOperationsMediator;
        _unmountedDriveService = unmountedDriveService;
        _mountedDriveService = mountedDriveService;
        _platformService = platformService;
    }

    public IDriveViewModel Create(DriveModel driveModel) =>
        new DriveViewModel(_fileSizeFormatter, _pathService, _filesOperationsMediator,
            _mountedDriveService, _platformService, driveModel);

    public IDriveViewModel Create(UnmountedDriveModel unmountedDriveModel) =>
        new UnmountedDriveViewModel(_unmountedDriveService, unmountedDriveModel);
}