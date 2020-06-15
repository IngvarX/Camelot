using System;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties
{
    public abstract class FileSystemNodeInformationDialogViewModelBase : ParameterizedDialogViewModelBase<FileSystemNodeNavigationParameter>
    {
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private string _name;
        private string _path;
        private long _size;
        private DateTime _createdDateTime;
        private DateTime _lastWriteDateTime;
        private DateTime _lastAccessDateTime;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string Path
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }

        public long Size
        {
            get => _size;
            set
            {
                this.RaiseAndSetIfChanged(ref _size, value);
                this.RaisePropertyChanged(nameof(FormattedSize));
            }
        }

        public string FormattedSize => _fileSizeFormatter.GetFormattedSize(Size);

        public DateTime CreatedDateTime
        {
            get => _createdDateTime;
            set => this.RaiseAndSetIfChanged(ref _createdDateTime, value);
        }

        public DateTime LastWriteDateTime
        {
            get => _lastWriteDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastWriteDateTime, value);
        }

        public DateTime LastAccessDateTime
        {
            get => _lastAccessDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastAccessDateTime, value);
        }

        protected FileSystemNodeInformationDialogViewModelBase(
            IFileSizeFormatter fileSizeFormatter)
        {
            _fileSizeFormatter = fileSizeFormatter;
        }
    }
}