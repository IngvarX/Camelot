using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.OperationsStates;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class OperationViewModelFactory : IOperationViewModelFactory
    {
        private readonly IPathService _pathService;

        public OperationViewModelFactory(IPathService pathService)
        {
            _pathService = pathService;
        }

        public IOperationViewModel Create(IOperation operation) =>
            new OperationViewModel(_pathService, operation);
    }
}