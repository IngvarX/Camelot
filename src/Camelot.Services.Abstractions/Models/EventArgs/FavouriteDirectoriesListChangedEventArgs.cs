namespace Camelot.Services.Abstractions.Models.EventArgs;

public class FavouriteDirectoriesListChangedEventArgs : System.EventArgs
{
    public string FullPath { get; }

    public FavouriteDirectoriesListChangedEventArgs(string fullPath)
    {
        FullPath = fullPath;
    }
}