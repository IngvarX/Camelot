using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.OperationsStates;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;

namespace Camelot.ViewModels.Factories.Implementations;

public class OperationStateViewModelFactory : IOperationStateViewModelFactory
{
    private readonly IPathService _pathService;

    public OperationStateViewModelFactory(IPathService pathService)
    {
        _pathService = pathService;
    }

    public IOperationStateViewModel Create(IOperation operation) =>
        new OperationStateViewModel(_pathService, operation);
}