using System;
using System.Windows.Input;
using Camelot.Extensions;
using Camelot.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class TabViewModel : ViewModelBase
    {
        private readonly IPathService _pathService;
        private bool _isActive;
        private string _currentDirectory;

        public string CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                this.RaiseAndSetIfChanged(ref _currentDirectory, value);
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

        public event EventHandler<EventArgs> CloseRequested;

        public event EventHandler<EventArgs> NewTabRequested;
        
        public ICommand ActivateCommand { get; }

        public TabViewModel(
            IPathService pathService,
            string directory)
        {
            _pathService = pathService;
            CurrentDirectory = directory;

            ActivateCommand = ReactiveCommand.Create(RequestActivation);
        }

        private void RequestActivation()
        {
            ActivationRequested.Raise(this, EventArgs.Empty);
        }
    }
}