using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface ITabViewModelFactory
    {
        ITabViewModel Create(TabStateModel tabModel);
    }
}