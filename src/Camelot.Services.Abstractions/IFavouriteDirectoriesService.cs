using System.Collections.Generic;

namespace Camelot.Services.Abstractions
{
    public interface IFavouriteDirectoriesService
    {
        IReadOnlyCollection<string> FavouriteDirectories { get; }

        void AddDirectory(string fullPath);

        void RemoveDirectory(string fullPath);
    }
}