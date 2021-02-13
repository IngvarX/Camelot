using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.AllPlatforms;
using Camelot.Services.Environment.Interfaces;
using Camelot.Services.Windows.Interfaces;

namespace Camelot.Services.Windows
{
    public class WindowsTrashCanService : TrashCanServiceBase
    {
        private const string FilePrefix = "$R";
        private const string MetadataPrefix = "$I";

        private readonly IPathService _pathService;
        private readonly IFileService _fileService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IWindowsRemovedFileMetadataBuilderFactory _removedFileMetadataBuilderFactory;
        private readonly IWindowsTrashCanNodeNameGenerator _trashCanNodeNameGenerator;
        private IDictionary<string, long> _fileSizesDictionary;
        private string _sid;

        public WindowsTrashCanService(
            IMountedDriveService mountedDriveService,
            IOperationsService operationsService,
            IPathService pathService,
            IFileService fileService,
            IDateTimeProvider dateTimeProvider,
            IProcessService processService,
            IWindowsRemovedFileMetadataBuilderFactory removedFileMetadataBuilderFactory,
            IWindowsTrashCanNodeNameGenerator trashCanNodeNameGenerator)
            : base(mountedDriveService, operationsService, pathService)
        {
            _pathService = pathService;
            _fileService = fileService;
            _dateTimeProvider = dateTimeProvider;
            _removedFileMetadataBuilderFactory = removedFileMetadataBuilderFactory;
            _trashCanNodeNameGenerator = trashCanNodeNameGenerator;

            InitializeAsync(processService).Forget();
        }

        protected override async Task PrepareAsync(IReadOnlyList<string> nodes)
        {
            var files = nodes.Where(_fileService.CheckIfExists).ToArray();
            _fileSizesDictionary = _fileService
                .GetFiles(files)
                .ToDictionary(f => f.FullPath, f => f.SizeBytes);

            await base.PrepareAsync(nodes);
        }

        protected override IReadOnlyList<string> GetTrashCanLocations(string volume) =>
            new[] {$"{volume}$Recycle.Bin\\{_sid}"};

        protected override string GetFilesTrashCanLocation(string trashCanLocation) => trashCanLocation;

        protected override async Task WriteMetaDataAsync(IReadOnlyDictionary<string, string> filePathsDictionary,
            string trashCanLocation)
        {
            var deleteTime = _dateTimeProvider.Now;

            foreach (var (originalFilePath, trashCanFilePath) in filePathsDictionary)
            {
                var fileSize = _fileSizesDictionary.ContainsKey(originalFilePath)
                    ? _fileSizesDictionary[originalFilePath]
                    : 0;
                var metadataBytes = GetMetadataBytes(originalFilePath, fileSize, deleteTime);
                var metadataFileName = _pathService
                    .GetFileName(trashCanFilePath)
                    .Replace(FilePrefix, MetadataPrefix);
                var metadataPath = _pathService.Combine(trashCanLocation, metadataFileName);

                await _fileService.WriteBytesAsync(metadataPath, metadataBytes);
            }
        }

        protected override string GetUniqueFilePath(string fileName, HashSet<string> filesNamesSet, string directory)
        {
            var extension = _pathService.GetExtension(fileName);
            var generatedName = _trashCanNodeNameGenerator.Generate();
            var newFileName = $"{FilePrefix}{generatedName}.{extension}";

            return _pathService.Combine(directory, newFileName);
        }

        protected override async Task CleanupAsync()
        {
            _fileSizesDictionary.Clear();

            await base.CleanupAsync();
        }

        private async Task InitializeAsync(IProcessService processService)
        {
            var userInfo = await processService.ExecuteAndGetOutputAsync("whoami", "/user");

            _sid = userInfo
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Last()
                .TrimEnd();
        }

        private byte[] GetMetadataBytes(string originalFilePath, long fileSize,
            DateTime removingDate)
        {
            var builder = CreateBuilder()
                .WithFileSize(fileSize)
                .WithRemovingDateTime(removingDate)
                .WithFilePath(originalFilePath);

            return builder.Build();
        }

        private IWindowsRemovedFileMetadataBuilder CreateBuilder() =>
            _removedFileMetadataBuilderFactory.Create();
    }
}