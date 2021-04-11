using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class DirectorySelectorViewModel : ViewModelBase, IDirectorySelectorViewModel
    {
        private readonly IFavouriteDirectoriesService _favouriteDirectoriesService;
        private readonly IDirectoryService _directoryService;
        private readonly ISuggestionsService _suggestionsService;
        private readonly ISuggestedPathViewModelFactory _suggestedPathViewModelFactory;

        private string _currentDirectory;
        private string _currentDirectorySearchText;

        private readonly ObservableCollection<ISuggestedPathViewModel> _suggestedPaths;

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                _currentDirectorySearchText = value;
                ClearSuggestions();

                if (!_directoryService.CheckIfExists(value))
                {
                    ReloadSuggestions();
                    ShouldShowSuggestions = SuggestedPaths.Any();

                    return;
                }

                this.RaiseAndSetIfChanged(ref _currentDirectory, value);

                ShouldShowSuggestions = false;
                UpdateFavouriteDirectoryStatus();

                CurrentDirectoryChanged.Raise(this, EventArgs.Empty);
            }
        }

        [Reactive]
        public bool ShouldShowSuggestions { get; set; }

        [Reactive]
        public bool IsFavouriteDirectory { get; set; }

        public IEnumerable<ISuggestedPathViewModel> SuggestedPaths => _suggestedPaths;

        public event EventHandler<EventArgs> CurrentDirectoryChanged;

        public ICommand ToggleFavouriteStatusCommand { get; }

        public DirectorySelectorViewModel(
            IFavouriteDirectoriesService favouriteDirectoriesService,
            IDirectoryService directoryService,
            ISuggestionsService suggestionsService,
            ISuggestedPathViewModelFactory suggestedPathViewModelFactory)
        {
            _favouriteDirectoriesService = favouriteDirectoriesService;
            _directoryService = directoryService;
            _suggestionsService = suggestionsService;
            _suggestedPathViewModelFactory = suggestedPathViewModelFactory;

            _suggestedPaths = new ObservableCollection<ISuggestedPathViewModel>();

            ToggleFavouriteStatusCommand = ReactiveCommand.Create(ToggleFavouriteStatus);

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _favouriteDirectoriesService.DirectoryAdded += (sender, args) => UpdateFavouriteDirectoryStatus();
            _favouriteDirectoriesService.DirectoryRemoved += (sender, args) => UpdateFavouriteDirectoryStatus();
        }

        private void ToggleFavouriteStatus()
        {
            if (IsFavouriteDirectory)
            {
                _favouriteDirectoriesService.AddDirectory(CurrentDirectory);
            }
            else
            {
                _favouriteDirectoriesService.RemoveDirectory(CurrentDirectory);
            }
        }

        private void ClearSuggestions() => _suggestedPaths.Clear();

        private void ReloadSuggestions()
        {
            var suggestions = _suggestionsService
                .GetSuggestions(_currentDirectorySearchText)
                .Select(sm => _suggestedPathViewModelFactory.Create(_currentDirectorySearchText, sm));

            _suggestedPaths.AddRange(suggestions);
        }

        private void UpdateFavouriteDirectoryStatus() => IsFavouriteDirectory =
            _favouriteDirectoriesService.ContainsDirectory(CurrentDirectory);
    }
}