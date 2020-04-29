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

        protected TrashCanServiceBase(
            IDriveService driveService,
            IOperationsService operationsService,
            IPathService pathService)
        {
            _driveService = driveService;
            _operationsService = operationsService;
            _pathService = pathService;
        }

        public async Task<bool> MoveToTrashAsync(IReadOnlyCollection<string> files)
        {
            var volume = GetVolume(files);
            var trashCanLocations = GetTrashCanLocations(volume);

            foreach (var trashCanLocation in trashCanLocations)
            {
                var filesTrashCanLocation = GetFilesTrashCanLocation(trashCanLocation);
                var destinationPaths = GetFilesTrashCanPathsMapping(files, filesTrashCanLocation);
                var isRemoved = await TryMoveToTrashAsync(destinationPaths);
                if (isRemoved)
                {
                    await WriteMetaDataAsync(destinationPaths, trashCanLocation);

                    return true;
                }
            }

            return false;
        }

        protected abstract IReadOnlyCollection<string> GetTrashCanLocations(string volume);

        protected abstract string GetFilesTrashCanLocation(string trashCanLocation);

        protected abstract Task WriteMetaDataAsync(IDictionary<string, string> files, string trashCanLocation);

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