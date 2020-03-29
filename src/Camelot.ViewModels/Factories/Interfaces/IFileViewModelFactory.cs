using Camelot.Services.Models;
using Camelot.ViewModels.Interfaces.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IFileSystemNodeViewModelFactory
    {
        IFileSystemNodeViewModel Create(FileModel fileModel);

        IFileSystemNodeViewModel Create(DirectoryModel directoryModel);
    }
}