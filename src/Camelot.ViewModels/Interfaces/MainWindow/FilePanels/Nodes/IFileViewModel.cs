namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;

public interface IFileViewModel : IFileSystemNodeViewModel
{
    long Size { get; }
}