using System.Windows.Input;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.MainWindow.Directories;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FavouriteDirectories
{
    public class FavouriteDirectoryViewModel : ViewModelBase, IFavouriteDirectoryViewModel
    {
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly DirectoryModel _directoryModel;

        public string DirectoryName => _directoryModel.Name;

        public ICommand OpenCommand { get; }

        public FavouriteDirectoryViewModel(
            IFilesOperationsMediator filesOperationsMediator,
            DirectoryModel directoryModel)
        {
            _filesOperationsMediator = filesOperationsMediator;
            _directoryModel = directoryModel;

            OpenCommand = ReactiveCommand.Create(Open);
        }

        private void Open() =>
            _filesOperationsMediator.ActiveFilesPanelViewModel.CurrentDirectory = _directoryModel.FullPath;
    }
}