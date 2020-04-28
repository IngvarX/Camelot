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

        protected TrashCanServiceBase(
            IDriveService driveService,
            IOperationsService operationsService)
        {
            _driveService = driveService;
            _operationsService = operationsService;
        }
        
        public async Task<bool> MoveToTrashAsync(IReadOnlyCollection<string> files)
        {
            var volume = GetVolume(files);
            var trashCanLocations = GetTrashCanLocations(volume);

            foreach (var trashCanLocation in trashCanLocations)
            {
                var filesTrashCanLocation = GetFilesTrashCanLocation(trashCanLocation);
                var isRemoved = await TryMoveToTrashAsync(files, filesTrashCanLocation);
                if (isRemoved)
                {
                    await WriteMetaDataAsync(files, trashCanLocation);
                    
                    return true;
                }
            }

            return false;
        }
        
        protected abstract IReadOnlyCollection<string> GetTrashCanLocations(string volume);
        
        protected abstract string GetFilesTrashCanLocation(string trashCanLocation);

        protected abstract Task WriteMetaDataAsync(IReadOnlyCollection<string> files, string trashCanLocation);
        
        private async Task<bool> TryMoveToTrashAsync(IReadOnlyCollection<string> files, string trashCanLocation)
        {
            await _operationsService.MoveFilesAsync(files, trashCanLocation);
            // TODO: check results in future
            return true;
        }

        private string GetVolume(IEnumerable<string> files) =>
            _driveService.GetFileDrive(files.First()).RootDirectory;
    }
}