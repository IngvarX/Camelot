using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Specifications
{
    public class EmptySpecification : ISpecification<NodeModelBase>
    {
        public bool IsSatisfiedBy(NodeModelBase nodeModel) => true;
    }
}