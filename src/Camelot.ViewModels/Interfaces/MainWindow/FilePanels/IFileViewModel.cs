namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IFileViewModel : IFileSystemNodeViewModel
    {
        long Size { get; }
    }
}