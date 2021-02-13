using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Drives;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.AllPlatforms
{
    public abstract class TrashCanServiceBase : ITrashCanService
    {
        private readonly IMountedDriveService _mountedDriveService;
        private readonly IOperationsService _operationsService;
        private readonly IPathService _pathService;

        protected TrashCanServiceBase(
            IMountedDriveService mountedDriveService,
            IOperationsService operationsService,
            IPathService pathService)
        {
            _mountedDriveService = mountedDriveService;
            _operationsService = operationsService;
            _pathService = pathService;
        }

        public async Task<bool> MoveToTrashAsync(IReadOnlyList<string> nodes, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var volume = GetVolume(nodes);

            await PrepareAsync(nodes);

            var trashCanLocations = GetTrashCanLocations(volume);
            var result = false;
            foreach (var trashCanLocation in trashCanLocations)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var filesTrashCanLocation = GetFilesTrashCanLocation(trashCanLocation); // TODO: create if not exists?
                var destinationPathsDictionary = GetFilesTrashCanPathsMapping(nodes, filesTrashCanLocation);
                await _operationsService.MoveAsync(destinationPathsDictionary);

                await WriteMetaDataAsync(destinationPathsDictionary, trashCanLocation);
                result = true;

                break;
            }

            await CleanupAsync();

            return result;
        }

        protected virtual Task PrepareAsync(IReadOnlyList<string> nodes) => Task.CompletedTask;

        protected abstract IReadOnlyList<string> GetTrashCanLocations(string volume);

        protected abstract string GetFilesTrashCanLocation(string trashCanLocation);

        protected abstract Task WriteMetaDataAsync(IReadOnlyDictionary<string, string> filePathsDictionary,
            string trashCanLocation);

        protected abstract string GetUniqueFilePath(string fileName, HashSet<string> filesNamesSet, string directory);

        protected virtual Task CleanupAsync() => Task.CompletedTask;

        private IReadOnlyDictionary<string, string> GetFilesTrashCanPathsMapping(IReadOnlyList<string> files,
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
            _mountedDriveService.GetFileDrive(files.First()).RootDirectory;
    }
}