using System;
using System.IO;
using System.Windows.Input;
using Camelot.Services.Behaviors.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class FileViewModel : ViewModelBase
    {
        private readonly IFileOpeningBehavior _fileOpeningBehavior;

        private string _lastModifiedDateTime;

        public string LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastModifiedDateTime, value);
        }

        public ICommand OpenCommand { get; }

        public string FullPath { get; }

        public string FileName => Path.GetFileName(FullPath);

        public FileViewModel(
            IFileOpeningBehavior fileOpeningBehavior,
            string fullPath,
            DateTime lastModifiedDateTime)
        {
            _fileOpeningBehavior = fileOpeningBehavior;
            FullPath = fullPath;
            LastModifiedDateTime = lastModifiedDateTime.ToString();
            OpenCommand = ReactiveCommand.Create(Open);
        }

        private void Open()
        {
            _fileOpeningBehavior.Open(FullPath);
        }
    }
}