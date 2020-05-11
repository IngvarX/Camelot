using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;

namespace Camelot.Services.Operations
{
    public class RemoveFileOperation : OperationBase
    {
        private readonly string _pathToRemove;
        private readonly IFileService _fileService;

        public RemoveFileOperation(
            string pathToRemove,
            IFileService fileService)
        {
            _pathToRemove = pathToRemove;
            _fileService = fileService;
        }

        public override Task RunAsync(CancellationToken cancellationToken)
        {
            _fileService.Remove(_pathToRemove);
            FireOperationFinishedEvent();

            return Task.CompletedTask;
        }
    }
}