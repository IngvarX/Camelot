using System.Collections.Generic;
using Camelot.Services.Abstractions;

namespace Camelot.Services
{
    public class FavouriteDirectoriesService : IFavouriteDirectoriesService
    {
        public ISet<string> FavouriteDirectories { get; }

        public FavouriteDirectoriesService()
        {
            FavouriteDirectories = new HashSet<string>();
        }
    }
}