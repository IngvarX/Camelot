using System.Collections.Generic;
using System.Collections.ObjectModel;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;

namespace Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories
{
    public class FavouriteDirectoriesListViewModel : ViewModelBase, IFavouriteDirectoriesListViewModel
    {
        private readonly IFavouriteDirectoryViewModelFactory _favouriteDirectoryViewModelFactory;
        private readonly ObservableCollection<IFavouriteDirectoryViewModel> _directories;

        public IEnumerable<IFavouriteDirectoryViewModel> Directories => _directories;

        public FavouriteDirectoriesListViewModel(
            IFavouriteDirectoryViewModelFactory favouriteDirectoryViewModelFactory,
            IHomeDirectoryProvider homeDirectoryProvider)
        {
            _favouriteDirectoryViewModelFactory = favouriteDirectoryViewModelFactory;
            _directories = new ObservableCollection<IFavouriteDirectoryViewModel>
            {
                CreateHomeDirectoryViewModel(homeDirectoryProvider)
            };
        }

        private IFavouriteDirectoryViewModel CreateHomeDirectoryViewModel(
            IHomeDirectoryProvider homeDirectoryProvider)
        {
            var homeDirectoryPath = homeDirectoryProvider.HomeDirectoryPath;

            return _favouriteDirectoryViewModelFactory.Create(homeDirectoryPath);
        }
    }
}