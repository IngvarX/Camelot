using System;
using System.IO;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class FileViewModel : ViewModelBase
    {
        private string _lastModifiedDateTime;

        public string LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastModifiedDateTime, value);
        }

        public string FullPath { get; }

        public string FileName => Path.GetFileName(FullPath);

        public FileViewModel(string fullPath, DateTime lastModifiedDateTime)
        {
            FullPath = fullPath;
            LastModifiedDateTime = lastModifiedDateTime.ToString();

        }
    }
}