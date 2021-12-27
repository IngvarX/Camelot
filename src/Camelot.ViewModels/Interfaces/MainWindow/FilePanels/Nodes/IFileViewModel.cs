using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes
{
    public interface IFileViewModel : IFileSystemNodeViewModel
    {
        long Size { get; }
        
        FileMimeType Type { get; }
    }
}