using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Linux.Interfaces.Builders;

namespace Camelot.Services.Linux
{
    public class LinuxTrashCanService : TrashCanServiceBase
    {
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILinuxRemovedFileMetadataBuilderFactory _removedFileMetadataBuilderFactory;
        private readonly IHomeDirectoryProvider _homeDirectoryProvider;
        private readonly IEnvironmentService _environmentService;

        public LinuxTrashCanService(
            IMountedDriveService mountedDriveService,
            IOperationsService operationsService,
            IPathService pathService,
            IFileService fileService,
            IEnvironmentService environmentService,
            IDirectoryService directoryService,
            IDateTimeProvider dateTimeProvider,
            ILinuxRemovedFileMetadataBuilderFactory removedFileMetadataBuilderFactory,
            IHomeDirectoryProvider homeDirectoryProvider)
            : base(mountedDriveService, operationsService, pathService)
        {
            _pathService = pathService;
            _fileService = fileService;
            _environmentService = environmentService;
            _directoryService = directoryService;
            _dateTimeProvider = dateTimeProvider;
            _removedFileMetadataBuilderFactory = removedFileMetadataBuilderFactory;
            _homeDirectoryProvider = homeDirectoryProvider;
        }

        protected override IReadOnlyList<string> GetTrashCanLocations(string volume)
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
            _pathService.Combine(trashCanLocation, "files");

        protected override async Task WriteMetaDataAsync(IReadOnlyDictionary<string, string> filePathsDictionary,
            string trashCanLocation)
        {
            var infoTrashCanLocation = GetInfoTrashCanLocation(trashCanLocation);
            if (!_directoryService.CheckIfExists(infoTrashCanLocation))
            {
                _directoryService.Create(infoTrashCanLocation);
            }

            var deleteTime = _dateTimeProvider.Now;

            await filePathsDictionary.ForEachAsync(kvp =>
                WriteMetaDataAsync(kvp.Key, kvp.Value, infoTrashCanLocation, deleteTime));
        }

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

        private async Task WriteMetaDataAsync(string oldFilePath, string newFilePath,
            string trashCanMetadataLocation, DateTime dateTime)
        {
            var fileName = _pathService.GetFileName(newFilePath);
            var metadataFullPath = _pathService.Combine(trashCanMetadataLocation, fileName + ".trashinfo");
            var metadata = GetMetadata(oldFilePath, dateTime);

            await _fileService.WriteTextAsync(metadataFullPath, metadata);
        }

        private string GetMetadata(string filePath, DateTime dateTime)
        {
            var builder = CreateBuilder()
                .WithFilePath(filePath)
                .WithRemovingDateTime(dateTime);

            return builder.Build();
        }

        private ILinuxRemovedFileMetadataBuilder CreateBuilder() =>
            _removedFileMetadataBuilderFactory.Create();

        private string GetInfoTrashCanLocation(string trashCanLocation) =>
            _pathService.Combine(trashCanLocation, "info");

        private string GetHomeTrashCanPath()
        {
            var xdgDataHome = _environmentService.GetEnvironmentVariable("XDG_DATA_HOME");
            if (xdgDataHome != null)
            {
                return _pathService.Combine(xdgDataHome, "Trash");
            }

            var home = _homeDirectoryProvider.HomeDirectoryPath;

            return _pathService.Combine(home, ".local/share/Trash");
        }

        private IReadOnlyList<string> GetVolumeTrashCanPaths(string volume)
        {
            var uid = GetUid();

            return new[]
            {
                _pathService.Combine(volume, $".Trash-{uid}"),
                _pathService.Combine(volume, $".Trash/{uid}")
            };
        }

        private string GetUid() => _environmentService.GetEnvironmentVariable("UID") ??
                                   _environmentService.GetEnvironmentVariable("KDE_SESSION_UID");
    }
}