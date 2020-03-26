using Camelot.Services.Models;
using Camelot.ViewModels.MainWindow;

namespace Camelot.Factories.Interfaces
{
    public interface IFileViewModelFactory
    {
        FileViewModel Create(FileModel fileModel);

        FileViewModel Create(DirectoryModel directoryModel);
    }
}