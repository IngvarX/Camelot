using System.Threading.Tasks;
using Camelot.ViewModels.MainWindow;

namespace Camelot.Mediator.Interfaces
{
    public interface IFilesOperationsMediator
    {
        void Register(FilesPanelViewModel activeFilesPanelViewModel, FilesPanelViewModel inactiveFilesPanelViewModel);

        void EditSelectedFiles();

        Task CopySelectedFilesAsync();

        Task MoveSelectedFilesAsync();

        void CreateNewDirectory(string directoryName);

        Task RemoveSelectedFilesAsync();
    }
}