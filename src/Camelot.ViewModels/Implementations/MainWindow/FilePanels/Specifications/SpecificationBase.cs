using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;

public abstract class SpecificationBase : INodeSpecification
{
    public bool IsRecursive { get; }

    protected SpecificationBase(bool isRecursive)
    {
        IsRecursive = isRecursive;
    }

    public abstract bool IsSatisfiedBy(NodeModelBase nodeModel);
}