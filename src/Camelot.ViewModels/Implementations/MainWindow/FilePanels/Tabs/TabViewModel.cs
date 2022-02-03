using System;
using System.Linq;
using System.Windows.Input;
using Camelot.Collections;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs;

public class TabViewModel : ViewModelBase, ITabViewModel
{
    private readonly IPathService _pathService;
    private readonly IDirectoryService _directoryService;
    private readonly IFilePanelDirectoryObserver _filePanelDirectoryObserver;

    private readonly LimitedSizeHistory<string> _history;

    private string _currentDirectory;

    public IFileSystemNodesSortingViewModel SortingViewModel { get; }

    public string CurrentDirectory
    {
        get => _currentDirectory;
        set
        {
            if (_currentDirectory == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _currentDirectory, value);
            this.RaisePropertyChanged(nameof(DirectoryName));

            AppendDirectoryToHistory(value);
        }
    }

    public string DirectoryName => _pathService.GetFileName(_pathService.RightTrimPathSeparators(CurrentDirectory));

    [Reactive]
    public bool IsActive { get; set; }

    [Reactive]
    public bool IsGloballyActive { get; set; }

    public event EventHandler<EventArgs> ActivationRequested;

    public event EventHandler<EventArgs> NewTabRequested;

    public event EventHandler<EventArgs> NewTabOnOppositePanelRequested;

    public event EventHandler<EventArgs> CloseRequested;

    public event EventHandler<EventArgs> ClosingTabsToTheLeftRequested;

    public event EventHandler<EventArgs> ClosingTabsToTheRightRequested;

    public event EventHandler<EventArgs> ClosingAllTabsButThisRequested;

    public event EventHandler<TabMoveRequestedEventArgs> MoveRequested;

    public ICommand ActivateCommand { get; }

    public ICommand NewTabCommand { get; }

    public ICommand NewTabOnOppositePanelCommand { get; }

    public ICommand CloseTabCommand { get; }

    public ICommand CloseTabsToTheLeftCommand { get; }

    public ICommand CloseTabsToTheRightCommand { get; }

    public ICommand CloseAllTabsButThisCommand { get; }

    public ICommand RequestMoveCommand { get; }

    public ICommand GoToPreviousDirectoryCommand { get; }

    public ICommand GoToNextDirectoryCommand { get; }

    public TabViewModel(
        IPathService pathService,
        IDirectoryService directoryService,
        IFilePanelDirectoryObserver filePanelDirectoryObserver,
        IFileSystemNodesSortingViewModel fileSystemNodesSortingViewModel,
        TabConfiguration tabConfiguration,
        TabStateModel tabStateModel)
    {
        _pathService = pathService;
        _directoryService = directoryService;
        _filePanelDirectoryObserver = filePanelDirectoryObserver;

        _history = new LimitedSizeHistory<string>(tabConfiguration.MaxHistorySize, tabStateModel.History,
            tabStateModel.CurrentPositionInHistory);
        _currentDirectory = tabStateModel.Directory;

        SortingViewModel = fileSystemNodesSortingViewModel;

        ActivateCommand = ReactiveCommand.Create(RequestActivation);
        NewTabCommand = ReactiveCommand.Create(RequestNewTab);
        NewTabOnOppositePanelCommand = ReactiveCommand.Create(RequestNewTabOnOppositePanel);
        CloseTabCommand = ReactiveCommand.Create(RequestClosing);
        CloseTabsToTheLeftCommand = ReactiveCommand.Create(RequestClosingTabsToTheLeft);
        CloseTabsToTheRightCommand = ReactiveCommand.Create(RequestClosingTabsToTheRight);
        CloseAllTabsButThisCommand = ReactiveCommand.Create(RequestClosingAllTabsButThis);
        RequestMoveCommand = ReactiveCommand.Create<ITabViewModel>(RequestMoveTo);
        GoToPreviousDirectoryCommand = ReactiveCommand.Create(GoToPreviousDirectory);
        GoToNextDirectoryCommand = ReactiveCommand.Create(GoToNextDirectory);
    }

    public TabStateModel GetState() => new()
    {
        Directory = CurrentDirectory,
        SortingSettings = GetSortingSettings(),
        History = _history.Items.ToList(),
        CurrentPositionInHistory = _history.CurrentIndex
    };

    private SortingSettingsStateModel GetSortingSettings() => new()
    {
        IsAscending = SortingViewModel.IsSortingByAscendingEnabled,
        SortingMode = SortingViewModel.SortingColumn
    };

    private void RequestActivation() => ActivationRequested.Raise(this, EventArgs.Empty);

    private void RequestNewTab() => NewTabRequested.Raise(this, EventArgs.Empty);

    private void RequestNewTabOnOppositePanel() => NewTabOnOppositePanelRequested.Raise(this, EventArgs.Empty);

    private void RequestClosing() => CloseRequested.Raise(this, EventArgs.Empty);

    private void RequestClosingTabsToTheLeft() => ClosingTabsToTheLeftRequested.Raise(this, EventArgs.Empty);

    private void RequestClosingTabsToTheRight() => ClosingTabsToTheRightRequested.Raise(this, EventArgs.Empty);

    private void RequestClosingAllTabsButThis() => ClosingAllTabsButThisRequested.Raise(this, EventArgs.Empty);

    private void RequestMoveTo(ITabViewModel target) => MoveRequested.Raise(this, new TabMoveRequestedEventArgs(target));

    private void GoToPreviousDirectory() => GoToDirectoryIfExists(() => _history.HasPrevious, _history.GoToPrevious);

    private void GoToNextDirectory() => GoToDirectoryIfExists(() => _history.HasNext, _history.GoToNext);

    private void GoToDirectoryIfExists(Func<bool> directoryExists, Func<string> getDirectory)
    {
        while (directoryExists())
        {
            var directory = getDirectory();
            if (_directoryService.CheckIfExists(directory))
            {
                SetDirectory(directory);
                break;
            }
        }
    }

    private void SetDirectory(string directory)
    {
        _filePanelDirectoryObserver.CurrentDirectory = _currentDirectory = directory;
        this.RaisePropertyChanged(nameof(DirectoryName));
        this.RaisePropertyChanged(nameof(CurrentDirectory));
    }

    private void AppendDirectoryToHistory(string directory) => _history.AddItem(directory);
}