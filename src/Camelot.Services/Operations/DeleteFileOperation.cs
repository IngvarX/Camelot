using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations
{
    public class DeleteFileOperation : OperationBase, IInternalOperation
    {
        private readonly string _fileToRemove;
        private readonly IFileService _fileService;

        public DeleteFileOperation(
            string fileToRemove,
            IFileService fileService)
        {
            _fileToRemove = fileToRemove;
            _fileService = fileService;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                State = OperationState.InProgress;
                _fileService.Remove(_fileToRemove);
                State = OperationState.Finished;
            }
            catch
            {
                // TODO: process exception
                State = OperationState.Cancelled;
            }
            finally
            {
                CurrentProgress = 1;
            }

            return Task.CompletedTask;
        }
    }
}