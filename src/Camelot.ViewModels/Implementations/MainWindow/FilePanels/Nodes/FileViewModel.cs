using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels.Nodes;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes
{
    public class FileViewModel : FileSystemNodeViewModelBase, IFileViewModel
    {
        private readonly IFileSizeFormatter _fileSizeFormatter;
        private readonly IFileTypeMapper _fileTypeMapper;
        private long _size;

        public string Extension { get; set; }

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

        public FileContentType Type => _fileTypeMapper.GetFileType(Extension);

        public FileViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
            IFileSystemNodeFacade fileSystemNodeFacade,
            bool shouldShowOpenSubmenu,
            IFileSizeFormatter fileSizeFormatter,
            IFileTypeMapper fileTypeMapper)
            : base(
                fileSystemNodeOpeningBehavior,
                fileSystemNodePropertiesBehavior,
                fileSystemNodeFacade,
                shouldShowOpenSubmenu)
        {
            _fileSizeFormatter = fileSizeFormatter;
            _fileTypeMapper = fileTypeMapper;
        }
    }
}