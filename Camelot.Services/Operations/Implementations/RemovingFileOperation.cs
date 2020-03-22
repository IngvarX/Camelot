using System.Threading.Tasks;
using Camelot.Services.Interfaces;

namespace Camelot.Services.Operations.Implementations
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
            _fileService.RemoveFile(pathToRemove);

            return Task.CompletedTask;
        }
    }
}