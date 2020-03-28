using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow
{
    public interface IFileSystemNodeViewModel
    {
        string FullPath { get; set; }
        
        string Name { get; set; }
        
        ICommand OpenCommand { get; }
    }
}