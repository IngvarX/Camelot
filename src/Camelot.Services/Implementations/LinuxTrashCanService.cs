using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class LinuxTrashCanService : TrashCanServiceBase
    {
        private readonly IEnvironmentService _environmentService;
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;

        public LinuxTrashCanService(
            IDriveService driveService,
            IOperationsService operationsService,
            IEnvironmentService environmentService,
            IPathService pathService,
            IFileService fileService)
            : base(driveService, operationsService)
        {
            _environmentService = environmentService;
            _pathService = pathService;
            _fileService = fileService;
        }

        protected override IReadOnlyCollection<string> GetTrashCanLocations(string volume)
        {
            var directories = new List<string>();
            if (volume != "/")
            {
                directories.AddRange(GetVolumeTrashCanPaths(volume));
            }
            
            directories.Add(GetHomeTrashCanPath());

            return directories;
        }

        protected override string GetFilesTrashCanLocation(string trashCanLocation) =>
            $"{trashCanLocation}/files";

        protected override async Task WriteMetaDataAsync(IReadOnlyCollection<string> files, string trashCanLocation)
        {
            var infoTrashCanLocation = GetInfoTrashCanLocation(trashCanLocation);
            var deleteTime = _environmentService.Now;

            await files.ForEachAsync(f => WriteMetaDataAsync(f, infoTrashCanLocation, deleteTime));
        }

        private async Task WriteMetaDataAsync(string file, string trashCanMetadataLocation, DateTime dateTime)
        {
            var fileName = _pathService.GetFileName(file);
            var metadataFullPath = _pathService.Combine(trashCanMetadataLocation, fileName + ".trashinfo");
            var metadata = GetMetadata(file, dateTime);

            await _fileService.WriteTextAsync(metadataFullPath, metadata);
        }

        private static string GetMetadata(string file, DateTime dateTime)
        {
            var stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine("[Trash Info]");
            stringBuilder.AppendLine($"Path={file}");
            stringBuilder.AppendLine($"DeletionDate={dateTime:s}");

            return stringBuilder.ToString();
        }
        
        private static string GetInfoTrashCanLocation(string trashCanLocation) =>
            $"{trashCanLocation}/info";

        private string GetHomeTrashCanPath()
        {
            var xdgDataHome = _environmentService.GetEnvironmentVariable("XDG_DATA_HOME");
            if (xdgDataHome != null)
            {
                return $"{xdgDataHome}/Trash/";
            }
            
            var home = _environmentService.GetEnvironmentVariable("HOME");

            return $"{home}/.local/share/Trash";
        }

        private IReadOnlyCollection<string> GetVolumeTrashCanPaths(string volume)
        {
            var uid = GetUid();

            return new[] {$"{volume}/.Trash/{uid}", $"{volume}/.Trash-{uid}"};
        }
        
        private string GetUid() => _environmentService.GetEnvironmentVariable("UID");
    }
}