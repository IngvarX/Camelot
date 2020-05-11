using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
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

        public MacTrashCanService(
            IDriveService driveService,
            IOperationsService operationsService,
            IPathService pathService,
            IFileService fileService,
            IEnvironmentService environmentService) 
            : base(driveService, operationsService, pathService, fileService)
        {
            _pathService = pathService;
            _fileService = fileService;
            _environmentService = environmentService;
        }

        protected override IReadOnlyCollection<string> GetTrashCanLocations(string volume)
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

        protected override Task WriteMetaDataAsync(IDictionary<string, string> filePathsDictionary,
            string trashCanLocation) => Task.CompletedTask;

        protected override string GetUniqueFilePath(string file, HashSet<string> filesSet, string directory)
        {
            // TODO: move to move operation
            var filePath = _pathService.Combine(directory, _pathService.GetFileName(file));
            if (!filesSet.Contains(filePath))
            {
                return filePath;
            }

            var fileName = _pathService.GetFileName(file);

            string result;
            var i = 1;
            do
            {
                var newFileName = $"{fileName} ({i})";
                result = _pathService.Combine(directory, newFileName);
                i++;
            } while (filesSet.Contains(result) || _fileService.CheckIfExists(result));

            return result;
        }
        
        private string GetHomeTrashCanPath()
        {
            var home = _environmentService.GetEnvironmentVariable("HOME");

            return $"{home}/.Trash";
        }

        private static string GetVolumeTrashCanPath(string volume) => $"{volume}/.Trashes";
    }
}