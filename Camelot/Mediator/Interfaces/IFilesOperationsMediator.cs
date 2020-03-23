using Camelot.ViewModels.MainWindow;

namespace Camelot.Mediator.Interfaces
{
    public interface IFilesOperationsMediator
    {
        string OutputDirectory { get; }

        void Register(FilesPanelViewModel activeFilesPanelViewModel, FilesPanelViewModel inactiveFilesPanelViewModel);
    }
}