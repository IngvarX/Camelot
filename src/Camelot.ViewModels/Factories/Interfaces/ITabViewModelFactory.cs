using Camelot.DataAccess.Models;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface ITabViewModelFactory
    {
        ITabViewModel Create(TabModel tabModel);
    }
}