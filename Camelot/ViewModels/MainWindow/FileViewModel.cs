using System;
using System.IO;
using Camelot.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class FileViewModel : ViewModelBase
    {
        private readonly Action _onClickedAction;
        private string _lastModifiedDateTime;

        public string LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastModifiedDateTime, value);
        }

        public string FullPath { get; }

        public string FileName => Path.GetFileName(FullPath);

        public FileViewModel(Action onClickedAction, string fullPath, DateTime lastModifiedDateTime)
        {
            _onClickedAction = onClickedAction;
            FullPath = fullPath;
            LastModifiedDateTime = lastModifiedDateTime.ToString();
        }

        public void Open()
        {
            _onClickedAction?.Invoke();
        }
    }
}