using Camelot.ViewModels.Implementations.MainWindow;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface ITabViewModelFactory
    {
        TabViewModel Create(string directory);
    }
}