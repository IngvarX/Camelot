using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface ISuggestedPathViewModelFactory
    {
        ISuggestedPathViewModel Create(string searchText, string fullPath);
    }
}