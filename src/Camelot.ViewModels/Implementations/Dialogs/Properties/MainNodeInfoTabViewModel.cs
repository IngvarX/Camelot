using System;
using Avalonia.Media.Imaging;
using Camelot.Services.Abstractions;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties
{
    public class MainNodeInfoTabViewModel : ViewModelBase
    {
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IPathService _pathService;
        private string _name;
        private string _path;
        private NodeType _type;
        private long _size;
        private DateTime _createdDateTime;
        private DateTime _lastWriteDateTime;
        private DateTime _lastAccessDateTime;

        public string Name => _pathService.GetFileName(FullPath);

        public string Path => _pathService.GetParentDirectory(FullPath);

        public string FullPath
        {
            get => _path;
            set => this.RaiseAndSetIfChanged(ref _path, value);
        }

        public NodeType Type
        {
            get => _type;
            set => this.RaiseAndSetIfChanged(ref _type, value);
        }

        public IBitmap ImageBitmap => Type == NodeType.Image ? new Bitmap(FullPath) : null;

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

        public MainNodeInfoTabViewModel(
            IFileSizeFormatter fileSizeFormatter,
            IPathService pathService)
        {
            _fileSizeFormatter = fileSizeFormatter;
            _pathService = pathService;
        }
    }
}