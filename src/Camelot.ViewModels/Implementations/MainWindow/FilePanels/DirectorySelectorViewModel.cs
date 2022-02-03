using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels;

public class DirectorySelectorViewModel : ViewModelBase, IDirectorySelectorViewModel
{
    private readonly IFavouriteDirectoriesService _favouriteDirectoriesService;
    private readonly IDirectoryService _directoryService;
    private readonly ISuggestionsService _suggestionsService;
    private readonly IFilePanelDirectoryObserver _filePanelDirectoryObserver;
    private readonly ISuggestedPathViewModelFactory _suggestedPathViewModelFactory;

    private string _currentDirectory;
    private string _currentDirectorySearchText;

    private readonly ObservableCollection<ISuggestedPathViewModel> _suggestedPaths;

    public string CurrentDirectory
    {
        get => _currentDirectory;
        set
        {
            if (_currentDirectorySearchText == value)
            {
                return;
            }

            _currentDirectorySearchText = value;
            ClearSuggestions();

            if (string.IsNullOrEmpty(value) || !_directoryService.CheckIfExists(value))
            {
                ReloadSuggestions();
                ShouldShowSuggestions = SuggestedPaths.Any();

                return;
            }

            this.RaiseAndSetIfChanged(ref _currentDirectory, value);

            ShouldShowSuggestions = false;
            UpdateFavouriteDirectoryStatus();

            _filePanelDirectoryObserver.CurrentDirectory = _currentDirectory;
        }
    }

    [Reactive]
    public bool ShouldShowSuggestions { get; set; }

    [Reactive]
    public bool IsFavouriteDirectory { get; set; }

    public event EventHandler<EventArgs> ActivationRequested;

    public IEnumerable<ISuggestedPathViewModel> SuggestedPaths => _suggestedPaths;

    public ICommand SaveFavouriteStatusCommand { get; }

    public ICommand ToggleFavouriteStatusCommand { get; }

    public DirectorySelectorViewModel(
        IFavouriteDirectoriesService favouriteDirectoriesService,
        IDirectoryService directoryService,
        ISuggestionsService suggestionsService,
        IFilePanelDirectoryObserver filePanelDirectoryObserver,
        ISuggestedPathViewModelFactory suggestedPathViewModelFactory)
    {
        _favouriteDirectoriesService = favouriteDirectoriesService;
        _directoryService = directoryService;
        _suggestionsService = suggestionsService;
        _filePanelDirectoryObserver = filePanelDirectoryObserver;
        _suggestedPathViewModelFactory = suggestedPathViewModelFactory;

        _suggestedPaths = new ObservableCollection<ISuggestedPathViewModel>();

        SaveFavouriteStatusCommand = ReactiveCommand.Create(SaveFavouriteStatus);
        ToggleFavouriteStatusCommand = ReactiveCommand.Create(ToggleFavouriteStatus);

        SubscribeToEvents();
    }

    public void Activate() => ActivationRequested.Raise(this, EventArgs.Empty);

    private void SubscribeToEvents()
    {
        _favouriteDirectoriesService.DirectoryAdded += (_, _) => UpdateFavouriteDirectoryStatus();
        _favouriteDirectoriesService.DirectoryRemoved += (_, _) => UpdateFavouriteDirectoryStatus();
        _filePanelDirectoryObserver.CurrentDirectoryChanged += (_, _) =>
            CurrentDirectory = _filePanelDirectoryObserver.CurrentDirectory;
    }

    private void ToggleFavouriteStatus()
    {
        IsFavouriteDirectory = !IsFavouriteDirectory;

        SaveFavouriteStatus();
    }

    private void SaveFavouriteStatus()
    {
        // IsFavouriteDirectory property changes before executing this command
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