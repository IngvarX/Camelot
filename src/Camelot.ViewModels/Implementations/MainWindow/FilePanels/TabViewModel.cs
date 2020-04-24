using System;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class TabViewModel : ViewModelBase, ITabViewModel
    {
        private readonly IPathService _pathService;

        private bool _isActive;
        private bool _isGloballyActive;
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

        public string DirectoryName => _pathService.GetFileName(_pathService.TrimPathSeparators(CurrentDirectory));

        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
        }

        public bool IsGloballyActive
        {
            get => _isGloballyActive;
            set => this.RaiseAndSetIfChanged(ref _isGloballyActive, value);
        }

        public event EventHandler<EventArgs> ActivationRequested;

        public event EventHandler<EventArgs> NewTabRequested;

        public event EventHandler<EventArgs> CloseRequested;

        public event EventHandler<EventArgs> ClosingTabsToTheLeftRequested;

        public event EventHandler<EventArgs> ClosingTabsToTheRightRequested;

        public event EventHandler<EventArgs> ClosingAllTabsButThisRequested;

        public ICommand ActivateCommand { get; }

        public ICommand NewTabCommand { get; }

        public ICommand CloseTabCommand { get; }

        public ICommand CloseTabsToTheLeftCommand { get; }

        public ICommand CloseTabsToTheRightCommand { get; }

        public ICommand CloseAllTabsButThisCommand { get; }

        public TabViewModel(
            IPathService pathService,
            IFileSystemNodesSortingViewModel fileSystemNodesSortingViewModel,
            string directory)
        {
            _pathService = pathService;
            SortingViewModel = fileSystemNodesSortingViewModel;
            CurrentDirectory = directory;

            ActivateCommand = ReactiveCommand.Create(RequestActivation);
            NewTabCommand = ReactiveCommand.Create(RequestNewTab);
            CloseTabCommand = ReactiveCommand.Create(RequestClosing);
            CloseTabsToTheLeftCommand = ReactiveCommand.Create(RequestClosingTabsToTheLeft);
            CloseTabsToTheRightCommand = ReactiveCommand.Create(RequestClosingTabsToTheRight);
            CloseAllTabsButThisCommand = ReactiveCommand.Create(RequestClosingAllTabsButThis);
        }

        private void RequestActivation() => ActivationRequested.Raise(this, EventArgs.Empty);

        private void RequestNewTab() => NewTabRequested.Raise(this, EventArgs.Empty);

        private void RequestClosing() => CloseRequested.Raise(this, EventArgs.Empty);

        private void RequestClosingTabsToTheLeft() => ClosingTabsToTheLeftRequested.Raise(this, EventArgs.Empty);

        private void RequestClosingTabsToTheRight() => ClosingTabsToTheRightRequested.Raise(this, EventArgs.Empty);

        private void RequestClosingAllTabsButThis() => ClosingAllTabsButThisRequested.Raise(this, EventArgs.Empty);
    }
}