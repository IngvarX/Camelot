using Camelot.Services.Abstractions.Specifications;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications.QuickSearch;

public class QuickSearchEmptySpecification : ISpecification<IFileSystemNodeViewModel>
{
    public bool IsSatisfiedBy(IFileSystemNodeViewModel node) => true;
}