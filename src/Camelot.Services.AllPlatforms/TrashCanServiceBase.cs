using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.AllPlatforms
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

        public async Task<bool> MoveToTrashAsync(IReadOnlyCollection<string> nodes, CancellationToken cancellationToken)
        {
            var volume = GetVolume(nodes);
            var files = nodes.Where(_fileService.CheckIfExists).ToArray();

            await PrepareAsync(files);

            var trashCanLocations = GetTrashCanLocations(volume);
            var result = false;
            foreach (var trashCanLocation in trashCanLocations)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var filesTrashCanLocation = GetFilesTrashCanLocation(trashCanLocation); // TODO: create if not exists?
                var destinationPathsDictionary = GetFilesTrashCanPathsMapping(nodes, filesTrashCanLocation);
                var isRemoved = await TryMoveToTrashAsync(destinationPathsDictionary);
                if (isRemoved)
                {
                    await WriteMetaDataAsync(destinationPathsDictionary, trashCanLocation);
                    result = true;

                    break;
                }
            }

            await CleanupAsync();

            return result;
        }
        
        protected virtual Task PrepareAsync(string[] files) => Task.CompletedTask;

        protected abstract IReadOnlyCollection<string> GetTrashCanLocations(string volume);

        protected abstract string GetFilesTrashCanLocation(string trashCanLocation);

        protected abstract Task WriteMetaDataAsync(IReadOnlyDictionary<string, string> filePathsDictionary,
            string trashCanLocation);

        protected abstract string GetUniqueFilePath(string fileName, HashSet<string> filesNamesSet, string directory);

        protected virtual Task CleanupAsync() => Task.CompletedTask;
        
        private async Task<bool> TryMoveToTrashAsync(IReadOnlyDictionary<string, string> files)
        {
            try
            {
                await _operationsService.MoveAsync(files);
            }
            catch
            {
                return false;
            }

            // TODO: check results in future
            return true;
        }

        private IReadOnlyDictionary<string, string> GetFilesTrashCanPathsMapping(IReadOnlyCollection<string> files,
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