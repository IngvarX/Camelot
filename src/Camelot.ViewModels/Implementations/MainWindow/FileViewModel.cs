using System.Windows.Input;
using Camelot.Services.Behaviors.Interfaces;
using Camelot.ViewModels.Interfaces.MainWindow;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow
{
    public class FileViewModel : ViewModelBase, IFileViewModel
    {
        private readonly IFileOpeningBehavior _fileOpeningBehavior;

        private string _lastModifiedDateTime;

        public string LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastModifiedDateTime, value);
        }

        public string FullPath { get; set; }

        public string FileName { get; set; }

        public string Extension { get; set; }

        public string Size { get; set; }

        public ICommand OpenCommand { get; }

        public FileViewModel(
            IFileOpeningBehavior fileOpeningBehavior)
        {
            _fileOpeningBehavior = fileOpeningBehavior;

            OpenCommand = ReactiveCommand.Create(Open);
        }

        private void Open()
        {
            _fileOpeningBehavior.Open(FullPath);
        }
    }
}