using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;
using Camelot.Services.Models;

namespace Camelot.Services
{
    public class RecursiveSearchService : IRecursiveSearchService
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;

        public RecursiveSearchService(
            IDirectoryService directoryService,
            IFileService fileService)
        {
            _directoryService = directoryService;
            _fileService = fileService;
        }

        public IRecursiveSearchResult Search(string directory, ISpecification<NodeModelBase> specification,
            CancellationToken cancellationToken)
        {
            Task TaskFactory(RecursiveSearchResult r) =>
                Task.Run(() =>
                {
                    ProcessFiles(directory, specification, r);
                    ProcessDirectories(directory, specification, r);
                }, cancellationToken);

            return new RecursiveSearchResult(TaskFactory);
        }

        private void ProcessFiles(string directory, ISpecification<NodeModelBase> specification,
            RecursiveSearchResult recursiveSearchResult)
        {
            foreach (var file in _directoryService.GetFilesRecursively(directory))
            {
                var model = _fileService.GetFile(file);
                if (specification.IsSatisfiedBy(model))
                {
                    recursiveSearchResult.RaiseNodeFoundEvent(model);
                }
            }
        }

        private void ProcessDirectories(string directory, ISpecification<NodeModelBase> specification,
            RecursiveSearchResult recursiveSearchResult)
        {
            foreach (var dir in _directoryService.GetDirectoriesRecursively(directory))
            {
                var model = _directoryService.GetDirectory(dir);
                if (specification.IsSatisfiedBy(model))
                {
                    recursiveSearchResult.RaiseNodeFoundEvent(model);
                }
            }
        }
    }
}