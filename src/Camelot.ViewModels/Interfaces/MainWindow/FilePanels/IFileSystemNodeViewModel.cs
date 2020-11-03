using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels
{
    public interface IFileSystemNodeViewModel
    {
        string FullPath { get; }

        string Name { get; set; }

        bool IsEditing { get; set; }

        bool IsWaitingForEdit { get; set; }

        ICommand OpenCommand { get; }
    }
}