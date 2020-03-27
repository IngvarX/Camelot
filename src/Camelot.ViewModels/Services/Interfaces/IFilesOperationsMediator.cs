using Camelot.ViewModels.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow;

namespace Camelot.ViewModels.Services.Interfaces
{
    public interface IFilesOperationsMediator
    {
        string OutputDirectory { get; }

        void Register(IFilesPanelViewModel activeFilesPanelViewModel, IFilesPanelViewModel inactiveFilesPanelViewModel);
    }
}