using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;

namespace Camelot.Services.Operations
{
    public class RemoveDirectoryOperation : OperationBase
    {
        private readonly string _pathToRemove;
        private readonly IDirectoryService _directoryService;

        public RemoveDirectoryOperation(
            string pathToRemove,
            IDirectoryService directoryService)
        {
            _pathToRemove = pathToRemove;
            _directoryService = directoryService;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            _directoryService.RemoveRecursively(_pathToRemove);
            FireOperationFinishedEvent();

            return Task.CompletedTask;
        }
    }
}