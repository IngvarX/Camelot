using System;
using System.Windows.Input;

namespace Camelot.ViewModels.Interfaces.MainWindow.Directories;

public interface IFavouriteDirectoryViewModel
{
    event EventHandler<FavouriteDirectoryMoveRequestedEventArgs> MoveRequested;

    ICommand RequestMoveCommand { get; }
}