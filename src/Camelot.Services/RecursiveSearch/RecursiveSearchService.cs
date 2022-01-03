using System;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.RecursiveSearch;
using Camelot.Services.Abstractions.Specifications;
using Microsoft.Extensions.Logging;

namespace Camelot.Services.RecursiveSearch;

public class RecursiveSearchService : IRecursiveSearchService
{
    private readonly IDirectoryService _directoryService;
    private readonly IFileService _fileService;
    private readonly IRecursiveSearchResultFactory _recursiveSearchResultFactory;
    private readonly ILogger _logger;

    public RecursiveSearchService(
        IDirectoryService directoryService,
        IFileService fileService,
        IRecursiveSearchResultFactory recursiveSearchResultFactory,
        ILogger logger)
    {
        _directoryService = directoryService;
        _fileService = fileService;
        _recursiveSearchResultFactory = recursiveSearchResultFactory;
        _logger = logger;
    }

    public IRecursiveSearchResult Search(string directory, ISpecification<NodeModelBase> specification,
        CancellationToken cancellationToken)
    {
        Task TaskFactory(INodeFoundEventPublisher publisher) =>
            Task.Run(() => ProcessNodes(directory, specification, publisher), cancellationToken);

        return _recursiveSearchResultFactory.Create(TaskFactory);
    }

    private void ProcessNodes(string directory, ISpecification<NodeModelBase> specification,
        INodeFoundEventPublisher publisher)
    {
        foreach (var node in _directoryService.GetNodesRecursively(directory))
        {
            try
            {
                if (_fileService.CheckIfExists(node))
                {
                    ProcessFile(node, specification, publisher);
                }
                else
                {
                    ProcessDirectory(node, specification, publisher);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during recursive search");
            }
        }
    }

    private void ProcessFile(string filePath, ISpecification<NodeModelBase> specification,
        INodeFoundEventPublisher publisher) =>
        ProcessNode(filePath, specification, publisher, _fileService.GetFile);

    private void ProcessDirectory(string directoryPath, ISpecification<NodeModelBase> specification,
        INodeFoundEventPublisher publisher) =>
        ProcessNode(directoryPath, specification, publisher, _directoryService.GetDirectory);

    private static void ProcessNode(string nodePath, ISpecification<NodeModelBase> specification,
        INodeFoundEventPublisher publisher, Func<string, NodeModelBase> factory)
    {
        var model = factory(nodePath);
        if (specification.IsSatisfiedBy(model))
        {
            publisher.RaiseNodeFoundEvent(nodePath);
        }
    }
}