using System;
using System.Collections.Generic;
using System.Linq;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;

namespace Camelot.Services;

public class FavouriteDirectoriesService : IFavouriteDirectoriesService
{
    private const string FavouriteDirectoriesKey = "FavouriteDirectories";

    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IPathService _pathService;
    private readonly IHomeDirectoryProvider _homeDirectoryProvider;

    private readonly List<string> _favouriteDirectories;

    public IReadOnlyCollection<string> FavouriteDirectories => _favouriteDirectories;

    public event EventHandler<FavouriteDirectoriesListChangedEventArgs> DirectoryAdded;

    public event EventHandler<FavouriteDirectoriesListChangedEventArgs> DirectoryRemoved;

    public FavouriteDirectoriesService(
        IUnitOfWorkFactory unitOfWorkFactory,
        IPathService pathService,
        IHomeDirectoryProvider homeDirectoryProvider)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _pathService = pathService;
        _homeDirectoryProvider = homeDirectoryProvider;

        _favouriteDirectories = GetFavouriteDirectories();
    }

    public void AddDirectory(string fullPath)
    {
        var preprocessedPath = PreprocessPath(fullPath);
        if (_favouriteDirectories.Contains(preprocessedPath))
        {
            return;
        }

        _favouriteDirectories.Add(preprocessedPath);
        SaveFavouriteDirectories();
        DirectoryAdded.Raise(this, CreateArgs(preprocessedPath));
    }

    public void RemoveDirectory(string fullPath)
    {
        var preprocessedPath = PreprocessPath(fullPath);
        if (_favouriteDirectories.Remove(preprocessedPath))
        {
            SaveFavouriteDirectories();
            DirectoryRemoved.Raise(this, CreateArgs(preprocessedPath));
        }
    }

    public void MoveDirectory(int fromIndex, int toIndex)
    {
        var path = _favouriteDirectories[fromIndex];

        _favouriteDirectories.RemoveAt(fromIndex);
        _favouriteDirectories.Insert(toIndex, path);

        SaveFavouriteDirectories();
    }

    public bool ContainsDirectory(string fullPath)
    {
        var preprocessedPath = PreprocessPath(fullPath);

        return _favouriteDirectories.Contains(preprocessedPath);
    }

    private List<string> GetFavouriteDirectories()
    {
        using var uow = _unitOfWorkFactory.Create();
        var repository = uow.GetRepository<FavouriteDirectories>();
        var directories = repository
            .GetById(FavouriteDirectoriesKey)
            ?.Directories
            ?.Select(d => d.FullPath) ?? GetDefaultDirectories();

        return directories.ToList();
    }

    private IEnumerable<string> GetDefaultDirectories() =>
        Enumerable.Repeat(_homeDirectoryProvider.HomeDirectoryPath, 1);

    private string PreprocessPath(string path) => _pathService.RightTrimPathSeparators(path);

    private void SaveFavouriteDirectories()
    {
        using var uow = _unitOfWorkFactory.Create();
        var repository = uow.GetRepository<FavouriteDirectories>();
        var directories = FavouriteDirectories.Select(CreateFrom).ToList();
        var dbEntity = CreateFrom(directories);

        repository.Upsert(FavouriteDirectoriesKey, dbEntity);
    }

    private static FavouriteDirectory CreateFrom(string fullPath) => new()
    {
        FullPath = fullPath
    };

    private static FavouriteDirectories CreateFrom(List<FavouriteDirectory> directories) => new()
    {
        Directories = directories
    };

    private static FavouriteDirectoriesListChangedEventArgs CreateArgs(string path) => new(path);
}