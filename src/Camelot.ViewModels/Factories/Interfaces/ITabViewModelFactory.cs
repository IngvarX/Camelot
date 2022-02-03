using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Interfaces;

public interface ITabViewModelFactory
{
    ITabViewModel Create(IFilePanelDirectoryObserver observer, TabStateModel tabModel);
}