using System;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class TabViewModel : ViewModelBase, ITabViewModel
    {
        private readonly IPathService _pathService;
        private bool _isActive;
        private string _currentDirectory;

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                var directory = _pathService.TrimPathSeparators(value);
                
                this.RaiseAndSetIfChanged(ref _currentDirectory, directory);
                this.RaisePropertyChanged(nameof(DirectoryName));
            }
        }

        public string DirectoryName => _pathService.GetFileName(CurrentDirectory);

        public bool IsActive
        {
            get => _isActive;
            set => this.RaiseAndSetIfChanged(ref _isActive, value);
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
            string directory)
        {
            _pathService = pathService;
            CurrentDirectory = directory;

            ActivateCommand = ReactiveCommand.Create(RequestActivation);
            NewTabCommand = ReactiveCommand.Create(RequestNewTab);
            CloseTabCommand = ReactiveCommand.Create(RequestClosing);
            CloseTabsToTheLeftCommand = ReactiveCommand.Create(RequestClosingTabsToTheLeft);
            CloseTabsToTheRightCommand = ReactiveCommand.Create(RequestClosingTabsToTheRight);
            CloseAllTabsButThisCommand = ReactiveCommand.Create(RequestClosingAllTabsButThis);
        }

        private void RequestActivation()
        {
            ActivationRequested.Raise(this, EventArgs.Empty);
        }
        
        private void RequestNewTab()
        {
            NewTabRequested.Raise(this, EventArgs.Empty);
        }
        
        private void RequestClosing()
        {
            CloseRequested.Raise(this, EventArgs.Empty);
        }
        
        private void RequestClosingTabsToTheLeft()
        {
            ClosingTabsToTheLeftRequested.Raise(this, EventArgs.Empty);
        }
        
        private void RequestClosingTabsToTheRight()
        {
            ClosingTabsToTheRightRequested.Raise(this, EventArgs.Empty);
        }

        private void RequestClosingAllTabsButThis()
        {
            ClosingAllTabsButThisRequested.Raise(this, EventArgs.Empty);
        }
    }
}