using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class FavouriteDirectoryViewModelFactory : IFavouriteDirectoryViewModelFactory
    {
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly IDirectoryService _directoryService;

        public FavouriteDirectoryViewModelFactory(
            IFilesOperationsMediator filesOperationsMediator,
            IDirectoryService directoryService)
        {
            _filesOperationsMediator = filesOperationsMediator;
            _directoryService = directoryService;
        }

        public IFavouriteDirectoryViewModel Create(string directory)
        {
            var directoryModel = _directoryService.GetDirectory(directory);

            return new FavouriteDirectoryViewModel(_filesOperationsMediator, directoryModel);
        }
    }
}