using System.Collections.Generic;
using System.Linq;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;

namespace Camelot.Services
{
    public class FavouriteDirectoriesService : IFavouriteDirectoriesService
    {
        private const string FavouriteDirectoriesKey = "FavouriteDirectories";

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPathService _pathService;

        private readonly HashSet<string> _favouriteDirectories;

        public IReadOnlyCollection<string> FavouriteDirectories => _favouriteDirectories;

        public FavouriteDirectoriesService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IPathService pathService)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _pathService = pathService;

            _favouriteDirectories = GetFavouriteDirectories();
        }

        public void AddDirectory(string fullPath)
        {
            var preprocessedPath = PreprocessPath(fullPath);
            if (_favouriteDirectories.Add(preprocessedPath))
            {
                SaveFavouriteDirectories();
            }
        }

        public void RemoveDirectory(string fullPath)
        {
            var preprocessedPath = PreprocessPath(fullPath);
            if (_favouriteDirectories.Remove(preprocessedPath))
            {
                SaveFavouriteDirectories();
            }
        }

        private HashSet<string> GetFavouriteDirectories()
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<FavouriteDirectories>();
            var directories = repository.GetById(FavouriteDirectoriesKey).Directories;

            return directories.Select(d => d.FullPath).ToHashSet();
        }

        private string PreprocessPath(string path) => _pathService.RightTrimPathSeparators(path);

        private void SaveFavouriteDirectories()
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<FavouriteDirectories>();
            var directories = FavouriteDirectories.Select(CreateFrom).ToList();
            var dbEntity = CreateFrom(directories);

            repository.Upsert(FavouriteDirectoriesKey, dbEntity);
        }

        private static FavouriteDirectory CreateFrom(string fullPath) => new FavouriteDirectory
        {
            FullPath = fullPath
        };

        private static FavouriteDirectories CreateFrom(List<FavouriteDirectory> directories) => new FavouriteDirectories
        {
            Directories = directories
        };
    }
}