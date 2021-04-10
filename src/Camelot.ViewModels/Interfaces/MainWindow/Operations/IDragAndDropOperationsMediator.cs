using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.ViewModels.Interfaces.MainWindow.Operations
{
    public interface IDragAndDropOperationsMediator
    {
        Task CopyFilesAsync(IReadOnlyList<string> files, string fullPath);

        Task MoveFilesAsync(IReadOnlyList<string> files, string fullPath);
    }
}