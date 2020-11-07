using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Operations;
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
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesOperationsMediator filesOperationsMediator,
            IFileSizeFormatter fileSizeFormatter,
            IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior,
            IDialogService dialogService,
            ITrashCanService trashCanService,
            IArchiveService archiveService,
            ISystemDialogService systemDialogService,
            IOpenWithApplicationService openWithApplicationService,
            IPathService pathService)
            : base(
                fileSystemNodeOpeningBehavior,
                operationsService,
                clipboardOperationsService,
                filesOperationsMediator,
                fileSystemNodePropertiesBehavior,
                dialogService,
                trashCanService,
                archiveService,
                systemDialogService,
                openWithApplicationService,
                pathService)
        {
            _fileSizeFormatter = fileSizeFormatter;
        }
    }
}