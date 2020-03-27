using Camelot.ViewModels.Interfaces.MainWindow;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface ITabViewModelFactory
    {
        ITabViewModel Create(string directory);
    }
}