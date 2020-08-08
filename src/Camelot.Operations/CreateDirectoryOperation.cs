using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
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
            _directoryService.Create(_directoryToCreate);

            return Task.CompletedTask;
        }
    }
}