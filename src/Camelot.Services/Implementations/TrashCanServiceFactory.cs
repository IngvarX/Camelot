using System;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class TrashCanServiceFactory : ITrashCanServiceFactory
    {
        private readonly IPlatformService _platformService;
        private readonly IDriveService _driveService;
        private readonly IOperationsService _operationsService;
        private readonly IEnvironmentService _environmentService;
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;

        public TrashCanServiceFactory(
            IPlatformService platformService,
            IDriveService driveService,
            IOperationsService operationsService,
            IEnvironmentService environmentService,
            IPathService pathService,
            IFileService fileService)
        {
            _platformService = platformService;
            _driveService = driveService;
            _operationsService = operationsService;
            _environmentService = environmentService;
            _pathService = pathService;
            _fileService = fileService;
        }

        public ITrashCanService Create()
        {
            var platform = _platformService.GetPlatform();
            switch (platform)
            {
                case Platform.Linux:
                    return new LinuxTrashCanService(_driveService, _operationsService, _environmentService,
                        _pathService, _fileService);
                case Platform.MacOs:
                case Platform.Windows:
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform));
            }
        }
    }
}