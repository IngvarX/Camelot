using System.Threading.Tasks;
using Camelot.Services.Abstractions;

namespace Camelot.Services.Operations
{
    public class RemovingFileOperation : RemovingOperationBase
    {
        private readonly IFileService _fileService;

        public RemovingFileOperation(
            string pathToRemove,
            IFileService fileService)
            : base(pathToRemove)
        {
            _fileService = fileService;
        }

        protected override Task RemoveAsync(string pathToRemove)
        {
            _fileService.Remove(pathToRemove);

            return Task.CompletedTask;
        }
    }
}