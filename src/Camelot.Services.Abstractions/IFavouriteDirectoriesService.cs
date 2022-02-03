using System;
using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services.Abstractions;

public interface IFavouriteDirectoriesService
{
    IReadOnlyCollection<string> FavouriteDirectories { get; }

    event EventHandler<FavouriteDirectoriesListChangedEventArgs> DirectoryAdded;

    event EventHandler<FavouriteDirectoriesListChangedEventArgs> DirectoryRemoved;

    void AddDirectory(string fullPath);

    void RemoveDirectory(string fullPath);

    void MoveDirectory(int fromIndex, int toIndex);

    bool ContainsDirectory(string fullPath);
}