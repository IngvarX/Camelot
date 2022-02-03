using System.Threading.Tasks;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Enums;

namespace Camelot.ViewModels.Services.Interfaces;

public interface IFileSystemNodeFacade
{
    Task PackAsync(string fullPath);

    Task OpenWithAsync(IFileSystemNodeOpeningBehavior behavior, string fullPath);

    Task ExtractAsync(ExtractCommandType commandType, string fullPath);

    bool Rename(string fullName, string fullPath);

    Task RenameInDialogAsync(string fullPath);

    Task CopyToClipboardAsync(string filePath);

    Task DeleteAsync(string filePath);

    Task CopyAsync(string filePath);

    Task MoveAsync(string filePath);

    bool CheckIfNodeIsArchive(string filePath);
}