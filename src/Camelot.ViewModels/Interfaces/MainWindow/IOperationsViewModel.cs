using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow
{
    public interface IOperationsViewModel
    {
        ICommand MoveToTrashCommand { get; }
    }
}