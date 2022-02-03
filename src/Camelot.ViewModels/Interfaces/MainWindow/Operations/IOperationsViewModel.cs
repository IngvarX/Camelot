using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.Operations;

public interface IOperationsViewModel
{
    ICommand MoveToTrashCommand { get; }
}