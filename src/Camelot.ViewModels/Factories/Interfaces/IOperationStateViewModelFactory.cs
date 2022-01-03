using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;

namespace Camelot.ViewModels.Factories.Interfaces;

public interface IOperationStateViewModelFactory
{
    IOperationStateViewModel Create(IOperation operation);
}