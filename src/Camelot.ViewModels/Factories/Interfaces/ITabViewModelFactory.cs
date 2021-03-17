using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface ITabViewModelFactory
    {
        ITabViewModel Create(TabStateModel tabModel);
    }
}