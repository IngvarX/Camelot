using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Operations
{
    public class CreateDirectoryOperation : OperationBase, IInternalOperation
    {
        private readonly string _directoryToCreate;
        private readonly IDirectoryService _directoryService;

        public CreateDirectoryOperation(
            string directoryToCreate,
            IDirectoryService directoryService)
        {
            _directoryToCreate = directoryToCreate;
            _directoryService = directoryService;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            State = OperationState.InProgress;
            var creationResult = _directoryService.Create(_directoryToCreate);
            State = creationResult ? OperationState.Finished : OperationState.Failed;
            SetFinalProgress();

            return Task.CompletedTask;
        }
    }
}