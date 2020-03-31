using System.Threading.Tasks;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Operations.Implementations
{
    public class RemovingDirectoryOperation : RemovingOperationBase
    {
        private readonly IDirectoryService _directoryService;

        public RemovingDirectoryOperation(
            string pathToRemove,
            IDirectoryService directoryService)
            : base(pathToRemove)
        {
            _directoryService = directoryService;
        }

        protected override Task RemoveAsync(string pathToRemove)
        {
            _directoryService.RemoveRecursively(pathToRemove);

            return Task.CompletedTask;
        }
    }
}