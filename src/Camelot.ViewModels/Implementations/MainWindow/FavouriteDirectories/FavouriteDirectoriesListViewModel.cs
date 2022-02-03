using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.EventArgs;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;

namespace Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;

public class FavouriteDirectoriesListViewModel : ViewModelBase, IFavouriteDirectoriesListViewModel
{
    private readonly IFavouriteDirectoryViewModelFactory _favouriteDirectoryViewModelFactory;
    private readonly IFavouriteDirectoriesService _favouriteDirectoriesService;

    private readonly IDictionary<string, IFavouriteDirectoryViewModel> _directoriesDictionary;
    private readonly ObservableCollection<IFavouriteDirectoryViewModel> _directories;

    public IEnumerable<IFavouriteDirectoryViewModel> Directories => _directories;

    public FavouriteDirectoriesListViewModel(
        IFavouriteDirectoryViewModelFactory favouriteDirectoryViewModelFactory,
        IFavouriteDirectoriesService favouriteDirectoriesService)
    {
        _favouriteDirectoryViewModelFactory = favouriteDirectoryViewModelFactory;
        _favouriteDirectoriesService = favouriteDirectoriesService;

        _directoriesDictionary = new Dictionary<string, IFavouriteDirectoryViewModel>();
        _directories = new ObservableCollection<IFavouriteDirectoryViewModel>(
            favouriteDirectoriesService.FavouriteDirectories.Select(CreateFrom));

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _favouriteDirectoriesService.DirectoryAdded += FavouriteDirectoriesServiceOnDirectoryAdded;
        _favouriteDirectoriesService.DirectoryRemoved += FavouriteDirectoriesServiceOnDirectoryRemoved;
    }

    private void FavouriteDirectoriesServiceOnDirectoryAdded(object sender, FavouriteDirectoriesListChangedEventArgs args)
    {
        var viewModel = CreateFrom(args.FullPath);

        _directories.Add(viewModel);
    }

    private void FavouriteDirectoriesServiceOnDirectoryRemoved(object sender, FavouriteDirectoriesListChangedEventArgs args)
    {
        var viewModel = _directoriesDictionary[args.FullPath];
        _directoriesDictionary.Remove(args.FullPath);

        UnsubscribeFromEvents(viewModel);
        _directories.Remove(viewModel);
    }

    private IFavouriteDirectoryViewModel CreateFrom(string fullPath)
    {
        var viewModel = _favouriteDirectoryViewModelFactory.Create(fullPath);
        _directoriesDictionary[fullPath] = viewModel;
        SubscribeToEvents(viewModel);

        return viewModel;
    }

    private void SubscribeToEvents(IFavouriteDirectoryViewModel model) =>
        model.MoveRequested += ModelOnMoveRequested;

    private void UnsubscribeFromEvents(IFavouriteDirectoryViewModel model) =>
        model.MoveRequested -= ModelOnMoveRequested;

    private void ModelOnMoveRequested(object sender, FavouriteDirectoryMoveRequestedEventArgs e)
    {
        var source = (IFavouriteDirectoryViewModel)sender;
        var target = e.Target;

        var sourceIndex = _directories.IndexOf(source);
        var targetIndex = _directories.IndexOf(target);

        _directories.RemoveAt(sourceIndex);
        _directories.Insert(targetIndex, source);

        _favouriteDirectoriesService.MoveDirectory(sourceIndex, targetIndex);
    }
}