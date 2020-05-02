using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public class FileViewModel : FileSystemNodeViewModelBase
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

        public string FormattedSize => _fileSizeFormatter.GetFormattedSize(_size);

        public FileViewModel(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesOperationsMediator filesOperationsMediator,
            IFileSizeFormatter fileSizeFormatter)
            : base(
                fileSystemNodeOpeningBehavior,
                operationsService,
                clipboardOperationsService,
                filesOperationsMediator)
        {
            _fileSizeFormatter = fileSizeFormatter;
        }
    }
}