using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Operations
{
    public class DeleteFileOperation : OperationBase, IInternalOperation
    {
        private readonly IFileService _fileService;
        private readonly string _fileToRemove;

        public DeleteFileOperation(
            IFileService fileService,
            string fileToRemove)
        {
            _fileService = fileService;
            _fileToRemove = fileToRemove;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            State = OperationState.InProgress;
            var isRemoved = _fileService.Remove(_fileToRemove);
            State = isRemoved ? OperationState.Finished : OperationState.Failed;
            SetFinalProgress();

            return Task.CompletedTask;
        }
    }
}