using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.ViewModels.Services.Interfaces;

public interface IDragAndDropOperationsMediator
{
    Task CopyFilesAsync(IReadOnlyList<string> files, string fullPath);

    Task MoveFilesAsync(IReadOnlyList<string> files, string fullPath);
}