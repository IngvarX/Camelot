using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Mediator.Interfaces;
using Camelot.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class OperationsViewModel : ViewModelBase
    {
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly IOperationsService _operationsService;
        private readonly IClipboardOperationsService _clipboardOperationsService;
        private readonly IFilesSelectionService _filesSelectionService;

        public ICommand EditCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand MoveCommand { get; }

        public ICommand NewDirectoryCommand { get; }

        public ICommand RemoveCommand { get; }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand PasteFromClipboardCommand { get; }

        public OperationsViewModel(
            IFilesOperationsMediator filesOperationsMediator,
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesSelectionService filesSelectionService)
        {
            _filesOperationsMediator = filesOperationsMediator;
            _operationsService = operationsService;
            _clipboardOperationsService = clipboardOperationsService;
            _filesSelectionService = filesSelectionService;

            EditCommand = ReactiveCommand.Create(Edit);
            CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
            MoveCommand = ReactiveCommand.CreateFromTask(MoveAsync);
            NewDirectoryCommand = ReactiveCommand.Create(CreateNewDirectory);
            RemoveCommand = ReactiveCommand.CreateFromTask(RemoveAsync);
            CopyToClipboardCommand = ReactiveCommand.CreateFromTask(CopyToClipboardAsync);
            PasteFromClipboardCommand = ReactiveCommand.CreateFromTask(PasteFromClipboardAsync);
        }

        private void Edit()
        {
            _operationsService.EditFiles(_filesSelectionService.SelectedFiles);
        }

        private Task CopyAsync()
        {
            return _operationsService.CopyFilesAsync(_filesSelectionService.SelectedFiles,
                _filesOperationsMediator.OutputDirectory);
        }

        private Task MoveAsync()
        {
            return _operationsService.MoveFilesAsync(_filesSelectionService.SelectedFiles,
                _filesOperationsMediator.OutputDirectory);
        }

        private void CreateNewDirectory()
        {
            //_filesOperationsMediator.CreateNewDirectory();
        }

        private Task RemoveAsync()
        {
            return _operationsService.RemoveFilesAsync(_filesSelectionService.SelectedFiles);
        }

        private Task CopyToClipboardAsync()
        {
            return _clipboardOperationsService.CopyFilesAsync(_filesSelectionService.SelectedFiles);
        }

        private Task PasteFromClipboardAsync()
        {
            return _clipboardOperationsService.PasteSelectedFilesAsync(_filesOperationsMediator.OutputDirectory);
        }
    }
}