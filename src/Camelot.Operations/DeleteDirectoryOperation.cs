using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Operations
{
    public class DeleteDirectoryOperation : OperationBase, IInternalOperation
    {
        private readonly IDirectoryService _directoryService;
        private readonly string _directoryToRemove;

        public DeleteDirectoryOperation(
            IDirectoryService directoryService,
            string directoryToRemove)
        {
            _directoryService = directoryService;
            _directoryToRemove = directoryToRemove;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            State = OperationState.InProgress;
            var isRemoved = _directoryService.RemoveRecursively(_directoryToRemove);
            State = isRemoved ? OperationState.Finished : OperationState.Failed;
            SetFinalProgress();

            return Task.CompletedTask;
        }
    }
}