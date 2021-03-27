using System.Collections.Generic;

namespace Camelot.Services.Abstractions
{
    public interface IFavouriteDirectoriesService
    {
        ISet<string> FavouriteDirectories { get; }
    }
}