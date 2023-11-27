using System;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Specifications;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications.QuickSearch;

public class QuickSearchSpecification : ISpecification<IFileSystemNodeViewModel>
{
    private readonly IPathService _pathService;
    private readonly string _searchTerm;

    public QuickSearchSpecification(IPathService pathService, string searchTerm)
    {
        _pathService = pathService;
        _searchTerm = searchTerm;
    }

    public bool IsSatisfiedBy(IFileSystemNodeViewModel node)
    {
        var name = _pathService.GetFileName(node.FullPath);

        return name.StartsWith(_searchTerm, StringComparison.OrdinalIgnoreCase);
    }
}