using System;
using System.IO;
using System.Windows.Input;
using Camelot.Extensions;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class TabViewModel : ViewModelBase
    {
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

        public string DirectoryName => Path.GetFileName(CurrentDirectory);

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
            string directory)
        {
            CurrentDirectory = directory;

            ActivateCommand = ReactiveCommand.Create(RequestActivation);
        }

        private void RequestActivation()
        {
            ActivationRequested.Raise(this, EventArgs.Empty);
        }
    }
}