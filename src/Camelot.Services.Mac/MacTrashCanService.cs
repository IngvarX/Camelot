using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services.Mac
{
    public class MacTrashCanService : TrashCanServiceBase
    {
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;
        private readonly IEnvironmentService _environmentService;
        private readonly IDirectoryService _directoryService;
        private readonly IHomeDirectoryProvider _homeDirectoryProvider;

        public MacTrashCanService(
            IMountedDriveService mountedDriveService,
            IOperationsService operationsService,
            IPathService pathService,
            IFileService fileService,
            IEnvironmentService environmentService,
            IDirectoryService directoryService,
            IHomeDirectoryProvider homeDirectoryProvider)
            : base(mountedDriveService, operationsService, pathService)
        {
            _pathService = pathService;
            _fileService = fileService;
            _environmentService = environmentService;
            _directoryService = directoryService;
            _homeDirectoryProvider = homeDirectoryProvider;
        }

        protected override IReadOnlyList<string> GetTrashCanLocations(string volume)
        {
            var directories = new List<string>();
            if (volume != "/")
            {
                directories.Add(GetVolumeTrashCanPath(volume));
            }

            directories.Add(GetHomeTrashCanPath());

            return directories;
        }

        protected override string GetFilesTrashCanLocation(string trashCanLocation) => trashCanLocation;

        protected override Task WriteMetaDataAsync(IReadOnlyDictionary<string, string> filePathsDictionary,
            string trashCanLocation) => Task.CompletedTask;

        protected override string GetUniqueFilePath(string fileName, HashSet<string> filesNamesSet, string directory)
        {
            var filePath = _pathService.Combine(directory, fileName);
            if (!filesNamesSet.Contains(filePath) && !CheckIfExists(filePath))
            {
                return filePath;
            }

            string result;
            var i = 1;
            do
            {
                var newFileName = $"{fileName} ({i})";
                result = _pathService.Combine(directory, newFileName);
                i++;
            } while (filesNamesSet.Contains(result) || CheckIfExists(result));

            return result;
        }

        private bool CheckIfExists(string nodePath) =>
            _fileService.CheckIfExists(nodePath) || _directoryService.CheckIfExists(nodePath);

        private string GetHomeTrashCanPath()
        {
            var homeDirectoryPath = _homeDirectoryProvider.HomeDirectoryPath;

            return _pathService.Combine(homeDirectoryPath, ".Trash");
        }

        private string GetVolumeTrashCanPath(string volume) => _pathService.Combine(volume, ".Trashes");
    }
}