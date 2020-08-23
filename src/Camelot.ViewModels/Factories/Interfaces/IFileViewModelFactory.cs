using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IFileSystemNodeViewModelFactory
    {
        IFileSystemNodeViewModel Create(string path);

        IFileSystemNodeViewModel Create(FileModel fileModel);

        IFileSystemNodeViewModel Create(DirectoryModel directoryModel, bool isParentDirectory = false);
    }
}