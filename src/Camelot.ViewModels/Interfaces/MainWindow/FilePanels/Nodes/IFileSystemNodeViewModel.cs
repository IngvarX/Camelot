using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;

public interface IFileSystemNodeViewModel
{
    string FullPath { get; }

    string Name { get; set; }

    bool IsEditing { get; set; }

    ICommand OpenCommand { get; }
}