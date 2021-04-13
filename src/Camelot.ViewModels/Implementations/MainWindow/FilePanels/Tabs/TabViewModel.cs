using System;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Tabs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs
{
    public class TabViewModel : ViewModelBase, ITabViewModel
    {
        private readonly IPathService _pathService;

        private string _currentDirectory;

        public IFileSystemNodesSortingViewModel SortingViewModel { get; }

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentDirectory, value);
                this.RaisePropertyChanged(nameof(DirectoryName));
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

        public event EventHandler<MoveRequestedEventArgs> MoveRequested;

        public ICommand ActivateCommand { get; }

        public ICommand NewTabCommand { get; }

        public ICommand NewTabOnOppositePanelCommand { get; }

        public ICommand CloseTabCommand { get; }

        public ICommand CloseTabsToTheLeftCommand { get; }

        public ICommand CloseTabsToTheRightCommand { get; }

        public ICommand CloseAllTabsButThisCommand { get; }

        public ICommand RequestMoveCommand { get; }

        public TabViewModel(
            IPathService pathService,
            IFileSystemNodesSortingViewModel fileSystemNodesSortingViewModel,
            TabStateModel tabStateModel)
        {
            _pathService = pathService;

            SortingViewModel = fileSystemNodesSortingViewModel;
            CurrentDirectory = tabStateModel.Directory;

            ActivateCommand = ReactiveCommand.Create(RequestActivation);
            NewTabCommand = ReactiveCommand.Create(RequestNewTab);
            NewTabOnOppositePanelCommand = ReactiveCommand.Create(RequestNewTabOnOppositePanel);
            CloseTabCommand = ReactiveCommand.Create(RequestClosing);
            CloseTabsToTheLeftCommand = ReactiveCommand.Create(RequestClosingTabsToTheLeft);
            CloseTabsToTheRightCommand = ReactiveCommand.Create(RequestClosingTabsToTheRight);
            CloseAllTabsButThisCommand = ReactiveCommand.Create(RequestClosingAllTabsButThis);
            RequestMoveCommand = ReactiveCommand.Create<ITabViewModel>(RequestMoveTo);
        }

        public TabStateModel GetState() => new TabStateModel
        {
            Directory = CurrentDirectory,
            SortingSettings = GetSortingSettings()
        };

        private SortingSettingsStateModel GetSortingSettings() =>
            new SortingSettingsStateModel
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

        private void RequestMoveTo(ITabViewModel target) => MoveRequested.Raise(this, new MoveRequestedEventArgs(target));
    }
}