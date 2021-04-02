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
                Task.Run(() => ProcessNodes(directory, specification, r), cancellationToken);

            return new RecursiveSearchResult(TaskFactory);
        }

        private void ProcessNodes(string directory, ISpecification<NodeModelBase> specification,
            RecursiveSearchResult recursiveSearchResult)
        {
            foreach (var node in _directoryService.GetNodesRecursively(directory))
            {
                if (_fileService.CheckIfExists(node))
                {
                    ProcessFile(node, specification, recursiveSearchResult);
                }
                else if (_directoryService.CheckIfExists(node))
                {
                    ProcessDirectory(node, specification, recursiveSearchResult);
                }
            }
        }

        private void ProcessFile(string filePath, ISpecification<NodeModelBase> specification,
            RecursiveSearchResult recursiveSearchResult)
        {
            var model = _fileService.GetFile(filePath);
            if (specification.IsSatisfiedBy(model))
            {
                recursiveSearchResult.RaiseNodeFoundEvent(model);
            }
        }

        private void ProcessDirectory(string filePath, ISpecification<NodeModelBase> specification,
            RecursiveSearchResult recursiveSearchResult)
        {
            var model = _directoryService.GetDirectory(filePath);
            if (specification.IsSatisfiedBy(model))
            {
                recursiveSearchResult.RaiseNodeFoundEvent(model);
            }
        }
    }
}