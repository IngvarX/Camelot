using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Implementations
{
    public abstract class TrashCanServiceBase : ITrashCanService
    {
        private readonly IDriveService _driveService;
        private readonly IOperationsService _operationsService;
        private readonly IPathService _pathService;
        private readonly IFileService _fileService;

        protected TrashCanServiceBase(
            IDriveService driveService,
            IOperationsService operationsService,
            IPathService pathService,
            IFileService fileService)
        {
            _driveService = driveService;
            _operationsService = operationsService;
            _pathService = pathService;
            _fileService = fileService;
        }

        public async Task<bool> MoveToTrashAsync(IReadOnlyCollection<string> files)
        {
            var volume = GetVolume(files);
            var filesSizeDictionary = _fileService
                .GetFiles(files)
                .ToDictionary(f => f.FullPath, f => f.SizeBytes);
            var trashCanLocations = GetTrashCanLocations(volume);

            foreach (var trashCanLocation in trashCanLocations)
            {
                var filesTrashCanLocation = GetFilesTrashCanLocation(trashCanLocation);
                var destinationPathsDictionary = GetFilesTrashCanPathsMapping(files, filesTrashCanLocation);
                var isRemoved = await TryMoveToTrashAsync(destinationPathsDictionary);
                if (isRemoved)
                {
                    await WriteMetaDataAsync(destinationPathsDictionary, filesSizeDictionary, trashCanLocation);

                    return true;
                }
            }

            return false;
        }

        protected abstract IReadOnlyCollection<string> GetTrashCanLocations(string volume);

        protected abstract string GetFilesTrashCanLocation(string trashCanLocation);

        protected abstract Task WriteMetaDataAsync(IDictionary<string, string> filePathsDictionary,
            IDictionary<string, long> fileSizesDictionary, string trashCanLocation);

        protected abstract string GetUniqueFilePath(string file, HashSet<string> filesSet, string directory);

        private async Task<bool> TryMoveToTrashAsync(IDictionary<string, string> files)
        {
            try
            {
                await _operationsService.MoveFilesAsync(files);
            }
            catch
            {
                return false;
            }

            // TODO: check results in future
            return true;
        }

        private IDictionary<string, string> GetFilesTrashCanPathsMapping(IReadOnlyCollection<string> files,
            string filesTrashCanLocation)
        {
            var fileNames = new HashSet<string>();
            var dictionary = new Dictionary<string, string>();
            foreach (var file in files)
            {
                var fileName = _pathService.GetFileName(file);
                var newFilePath = GetUniqueFilePath(fileName, fileNames, filesTrashCanLocation);
                fileNames.Add(newFilePath);

                dictionary.Add(file, newFilePath);
            }

            return dictionary;
        }

        private string GetVolume(IEnumerable<string> files) =>
            _driveService.GetFileDrive(files.First()).RootDirectory;
    }
}