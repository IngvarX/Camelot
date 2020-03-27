using Camelot.ViewModels.MainWindow;

namespace Camelot.ViewModels.Services.Interfaces
{
    public interface IFilesOperationsMediator
    {
        string OutputDirectory { get; }

        void Register(FilesPanelViewModel activeFilesPanelViewModel, FilesPanelViewModel inactiveFilesPanelViewModel);
    }
}