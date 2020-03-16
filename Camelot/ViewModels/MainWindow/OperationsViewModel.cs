using System.Threading.Tasks;
using Camelot.Mediator.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.MainWindow
{
    public class OperationsViewModel : ViewModelBase
    {
        private readonly IFilesOperationsMediator _filesOperationsMediator;

        public IReactiveCommand EditCommand { get; set; }

        public IReactiveCommand CopyCommand { get; set; }

        public IReactiveCommand MoveCommand { get;  set;}

        public IReactiveCommand NewDirectoryCommand { get; set; }

        public IReactiveCommand RemoveCommand { get; set; }

        public OperationsViewModel(
            IFilesOperationsMediator filesOperationsMediator)
        {
            _filesOperationsMediator = filesOperationsMediator;

            EditCommand = ReactiveCommand.Create(Edit);
            CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
            MoveCommand = ReactiveCommand.CreateFromTask(MoveAsync);
            NewDirectoryCommand = ReactiveCommand.Create(CreateNewDirectory);
            RemoveCommand = ReactiveCommand.CreateFromTask(RemoveAsync);
        }

        private void Edit()
        {
            _filesOperationsMediator.EditSelectedFiles();
        }

        private Task CopyAsync()
        {
            return _filesOperationsMediator.CopySelectedFilesAsync();
        }

        private Task MoveAsync()
        {
            return _filesOperationsMediator.MoveSelectedFilesAsync();
        }

        private void CreateNewDirectory()
        {
            //_filesOperationsMediator.CreateNewDirectory();
        }

        private Task RemoveAsync()
        {
            return _filesOperationsMediator.RemoveSelectedFilesAsync();
        }
    }
}