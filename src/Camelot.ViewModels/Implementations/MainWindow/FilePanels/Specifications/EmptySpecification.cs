using Camelot.Services.Abstractions.Models;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications;

public class EmptySpecification : SpecificationBase
{
    public EmptySpecification(bool isRecursive)
        : base(isRecursive)
    {

    }

    public override bool IsSatisfiedBy(NodeModelBase nodeModel) => true;
}