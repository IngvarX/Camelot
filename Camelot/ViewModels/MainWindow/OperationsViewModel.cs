using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Interfaces;
using Camelot.Services.Operations.Settings;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class OperationsViewModel : ViewModelBase
    {
        private readonly IFilesSelectionService _filesSelectionService;
        private readonly IOperationsFactory _operationsFactory;

        public IReactiveCommand EditCommand { get; set; }

        public IReactiveCommand CopyCommand { get; set; }

        public IReactiveCommand MoveCommand { get;  set;}

        public IReactiveCommand NewFolderCommand { get; set; }

        public IReactiveCommand RemoveCommand { get; set; }

        public OperationsViewModel(
            IFilesSelectionService filesSelectionService,
            IOperationsFactory operationsFactory)
        {
            _filesSelectionService = filesSelectionService;
            _operationsFactory = operationsFactory;

            EditCommand = ReactiveCommand.Create(Edit);
            CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
            MoveCommand = ReactiveCommand.Create(Move);
            NewFolderCommand = ReactiveCommand.Create(CreateNewFolder);
            RemoveCommand = ReactiveCommand.CreateFromTask(RemoveAsync);
        }

        private void Edit()
        {
            throw new System.NotImplementedException();
        }

        private Task CopyAsync()
        {
            return Task.CompletedTask;
        }

        private void Move()
        {
            throw new System.NotImplementedException();
        }

        private void CreateNewFolder()
        {
            throw new System.NotImplementedException();
        }

        private async Task RemoveAsync()
        {
            var selectedFiles = _filesSelectionService
                .SelectedFiles
                .Select(f => new UnaryFileOperationSettings(f))
                .ToArray();
            if (!selectedFiles.Any())
            {
                return;
            }

            var deleteOperation = _operationsFactory.CreateDeleteOperation(selectedFiles);

            await deleteOperation.RunAsync();
        }
    }
}