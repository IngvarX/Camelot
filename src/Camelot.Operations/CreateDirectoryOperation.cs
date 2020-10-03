using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Operations
{
    public class CreateDirectoryOperation : OperationBase, IInternalOperation
    {
        private readonly IDirectoryService _directoryService;
        private readonly string _directoryToCreate;

        public CreateDirectoryOperation(
            IDirectoryService directoryService,
            string directoryToCreate)
        {
            _directoryService = directoryService;
            _directoryToCreate = directoryToCreate;
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