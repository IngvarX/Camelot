using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow
{
    public interface IFileSystemNodeViewModel
    {
        string FullPath { get; }
        
        ICommand OpenCommand { get; }
    }
}