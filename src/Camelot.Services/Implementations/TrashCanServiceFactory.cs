using System;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Extensions;
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
        private readonly IProcessService _processService;
        private readonly IDirectoryService _directoryService;

        private string _sid;

        public TrashCanServiceFactory(
            IPlatformService platformService,
            IDriveService driveService,
            IOperationsService operationsService,
            IEnvironmentService environmentService,
            IPathService pathService,
            IFileService fileService,
            IProcessService processService,
            IDirectoryService directoryService)
        {
            _platformService = platformService;
            _driveService = driveService;
            _operationsService = operationsService;
            _environmentService = environmentService;
            _pathService = pathService;
            _fileService = fileService;
            _processService = processService;
            _directoryService = directoryService;

            InitializeAsync().Forget();
        }

        public ITrashCanService Create()
        {
            var platform = _platformService.GetPlatform();
            switch (platform)
            {
                case Platform.Linux:
                    return new LinuxTrashCanService(_driveService, _operationsService,
                        _pathService, _environmentService, _fileService, _directoryService);
                case Platform.Windows:
                    return new WindowsTrashCanService(_driveService, _operationsService, _pathService,
                        _fileService, _environmentService, _sid);
                case Platform.MacOs:
                default:
                    throw new ArgumentOutOfRangeException(nameof(platform));
            }
        }

        private async Task InitializeAsync()
        {
            if (_platformService.GetPlatform() != Platform.Windows)
            {
                return;
            }

            var userInfo = await _processService.ExecuteAndGetOutputAsync("whoami", "/user");

            _sid = userInfo.Split(" ", StringSplitOptions.RemoveEmptyEntries).Last().TrimEnd();
        }
    }
}