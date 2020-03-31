using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IFileSystemNodeViewModel
    {
        string FullPath { get; set; }
        
        string Name { get; set; }
        
        bool IsEditing { get; set; }
        
        ICommand OpenCommand { get; }
    }
}