using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Specifications;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public interface INodeSpecification : ISpecification<NodeModelBase>
{
    bool IsRecursive { get; }
}