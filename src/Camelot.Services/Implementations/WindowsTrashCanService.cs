using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Builders;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public class WindowsTrashCanService : TrashCanServiceBase
    {
        private const string FilePrefix = "$R";
        private const string MetadataPrefix = "$I";
        private const string FileNameChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int FileNameLength = 6;

        private readonly IPathService _pathService;
        private readonly IFileService _fileService;
        private readonly IEnvironmentService _environmentService;
        private readonly string _sid;
        private readonly Random _random;

        public WindowsTrashCanService(
            IDriveService driveService,
            IOperationsService operationsService,
            IPathService pathService,
            IFileService fileService,
            IEnvironmentService environmentService,
            string sid)
            : base(driveService, operationsService, pathService)
        {
            _pathService = pathService;
            _fileService = fileService;
            _environmentService = environmentService;
            _sid = sid;

            _random = new Random();
        }

        protected override IReadOnlyCollection<string> GetTrashCanLocations(string volume) =>
            new[] {$"{volume}$Recycle.Bin\\{_sid}"};

        protected override string GetFilesTrashCanLocation(string trashCanLocation) => trashCanLocation;

        protected override async Task WriteMetaDataAsync(IDictionary<string, string> files, string trashCanLocation)
        {
            var deleteTime = _environmentService.Now;

            foreach (var (originalFilePath, trashCanFilePath) in files)
            {
                var metadataBytes = GetMetadataBytes(originalFilePath, trashCanFilePath, deleteTime);
                var metadataFileName = _pathService.GetFileName(trashCanFilePath).Replace(FilePrefix, MetadataPrefix);
                var metadataPath = _pathService.Combine(trashCanLocation, metadataFileName);

                await _fileService.WriteBytesAsync(metadataPath, metadataBytes);
            }
        }

        protected override string GetUniqueFilePath(string file, HashSet<string> filesSet, string directory)
        {
            var extension = _pathService.GetExtension(file);
            var generatedName = GenerateName();
            var fileName = $"{FilePrefix}{generatedName}.{extension}";

            return _pathService.Combine(directory, fileName);
        }

        private byte[] GetMetadataBytes(string originalFilePath, string trashCanFilePath, DateTime removingDate)
        {
            var fileModel = _fileService.GetFile(trashCanFilePath);
            var builder = new WindowsRemovedFileMetadataBuilder()
                .WithFileSize(fileModel.SizeBytes)
                .WithRemovingDateTime(removingDate)
                .WithFilePath(originalFilePath);

            return builder.Build();
        }

        private string GenerateName() => new string(
            Enumerable
                .Repeat(FileNameChars, FileNameLength)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray()
        );
    }
}