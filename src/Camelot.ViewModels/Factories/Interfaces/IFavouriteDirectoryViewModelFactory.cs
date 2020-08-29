using Camelot.ViewModels.Interfaces.MainWindow.Directories;

namespace Camelot.ViewModels.Factories.Interfaces
{
    public interface IFavouriteDirectoryViewModelFactory
    {
        IFavouriteDirectoryViewModel Create(string directory);
    }
}