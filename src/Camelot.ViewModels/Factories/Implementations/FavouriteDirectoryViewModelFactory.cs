using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations;

public class FavouriteDirectoryViewModelFactory : IFavouriteDirectoryViewModelFactory
{
    private readonly IFilesOperationsMediator _filesOperationsMediator;
    private readonly IDirectoryService _directoryService;
    private readonly IFavouriteDirectoriesService _favouriteDirectoriesService;

    public FavouriteDirectoryViewModelFactory(
        IFilesOperationsMediator filesOperationsMediator,
        IDirectoryService directoryService,
        IFavouriteDirectoriesService favouriteDirectoriesService)
    {
        _filesOperationsMediator = filesOperationsMediator;
        _directoryService = directoryService;
        _favouriteDirectoriesService = favouriteDirectoriesService;
    }

    public IFavouriteDirectoryViewModel Create(string directory)
    {
        var directoryModel = _directoryService.GetDirectory(directory);

        return new FavouriteDirectoryViewModel(_filesOperationsMediator, _favouriteDirectoriesService,
            directoryModel);
    }
}