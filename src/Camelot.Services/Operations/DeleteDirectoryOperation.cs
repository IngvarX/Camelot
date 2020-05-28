using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public class DeleteDirectoryOperation : OperationBase, IInternalOperation
    {
        private readonly string _directoryToRemove;
        private readonly IDirectoryService _directoryService;

        public DeleteDirectoryOperation(
            string directoryToRemove,
            IDirectoryService directoryService)
        {
            _directoryToRemove = directoryToRemove;
            _directoryService = directoryService;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                State = OperationState.InProgress;
                _directoryService.RemoveRecursively(_directoryToRemove);
                State = OperationState.Finished;
            }
            catch
            {
                // TODO: process exception
                State = OperationState.Cancelled;
            }

            return Task.CompletedTask;
        }
    }
}