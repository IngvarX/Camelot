using Camelot.ViewModels.MainWindow;

namespace Camelot.Factories.Interfaces
{
    public interface ITabViewModelFactory
    {
        TabViewModel Create(string directory);
    }
}