using Camelot.Services.Models;
using Camelot.ViewModels.Implementations.MainWindow;
using Camelot.ViewModels.Interfaces.MainWindow;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IFileViewModelFactory
    {
        IFileViewModel Create(FileModel fileModel);

        IFileViewModel Create(DirectoryModel directoryModel);
    }
}