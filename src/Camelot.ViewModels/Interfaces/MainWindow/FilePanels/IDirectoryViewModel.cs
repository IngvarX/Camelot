namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IDirectoryViewModel : IFileSystemNodeViewModel
    {
        bool IsParentDirectory { get; set; }
    }
}