using System;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class FileViewModel : ViewModelBase
    {
        private string _lastModifiedDateTime;
        private string _fileName;

        public string LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastModifiedDateTime, value);
        }

        public string FileName
        {
            get => _fileName;
            set => this.RaiseAndSetIfChanged(ref _fileName, value);
        }

        public FileViewModel(string fileName, DateTime lastModifiedDateTime)
        {
            _fileName = fileName;

            LastModifiedDateTime = lastModifiedDateTime.ToString();

        }
    }
}