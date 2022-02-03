using System;

namespace Camelot.ViewModels.Interfaces.MainWindow.Directories;

public class FavouriteDirectoryMoveRequestedEventArgs : EventArgs
{
    public IFavouriteDirectoryViewModel Target { get; }

    public FavouriteDirectoryMoveRequestedEventArgs(IFavouriteDirectoryViewModel target)
    {
        Target = target;
    }
}