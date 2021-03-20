using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class FileViewModel : FileSystemNodeViewModelBase, IFileViewModel
    {
        private readonly IFileSizeFormatter _fileSizeFormatter;
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

        public FileViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
            IFileSystemNodeFacade fileSystemNodeFacade,
            bool shouldShowOpenSubmenu,
            bool isArchive,
            IFileSizeFormatter fileSizeFormatter)
            : base(
                fileSystemNodeOpeningBehavior,
                fileSystemNodePropertiesBehavior,
                fileSystemNodeFacade,
                shouldShowOpenSubmenu,
                isArchive)
        {
            _fileSizeFormatter = fileSizeFormatter;
        }
    }
}