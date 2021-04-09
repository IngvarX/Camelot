using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow
{
    public interface IOperationsViewModel
    {
        ICommand MoveToTrashCommand { get; }

        Task PasteFilesAsync(IReadOnlyList<string> files, string fullPath);
    }
}