using System.Windows.Input;
using Camelot.Services.Behaviors.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public abstract class FileSystemNodeViewModelBase : ViewModelBase, IFileSystemNodeViewModel
    {
        private readonly IFileSystemNodeOpeningBehavior _fileSystemNodeOpeningBehavior;
        
        private string _lastModifiedDateTime;
        private string _fullPath;

        public string LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastModifiedDateTime, value);
        }

        public string FullPath
        {
            get => _fullPath;
            set => this.RaiseAndSetIfChanged(ref _fullPath, value);
        }
        
        public ICommand OpenCommand { get; }

        protected FileSystemNodeViewModelBase(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior)
        {
            _fileSystemNodeOpeningBehavior = fileSystemNodeOpeningBehavior;
            _fileSystemNodeOpeningBehavior = fileSystemNodeOpeningBehavior;

            OpenCommand = ReactiveCommand.Create(Open);
        }

        private void Open()
        {
            _fileSystemNodeOpeningBehavior.Open(FullPath);
        }
    }
}