using Camelot.ViewModels.MainWindow;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface ITabViewModelFactory
    {
        TabViewModel Create(string directory);
    }
}