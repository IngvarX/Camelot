using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Interfaces.MainWindow.OperationsStates;

public interface IOperationStateViewModel
{
    OperationState State { get; }
}