using System;
using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.Directories
{
    public interface IFavouriteDirectoryViewModel
    {
        ICommand RequestMoveCommand { get; }

        event EventHandler<FavouriteDirectoryMoveRequestedEventArgs> MoveRequested;
    }
}