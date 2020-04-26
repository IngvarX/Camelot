using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Camelot.ViewModels.Implementations.NavigationParameters;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class RemoveNodesConfirmationDialogViewModel : ParameterizedDialogViewModelBase<bool, NodesRemovingNavigationParameter>
    {
        private const int ShowedFilesLimit = 4;
        
        private IEnumerable<string> _files;
        
        public IEnumerable<string> Files
        {
            get => _files;
            set => this.RaiseAndSetIfChanged(ref _files, value);
        }

        public int FilesCount => Files.Count();

        public bool ShouldShowFilesList => FilesCount <= ShowedFilesLimit;
        
        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }
        
        public RemoveNodesConfirmationDialogViewModel()
        {
            OkCommand = ReactiveCommand.Create(() => Close(true));
            CancelCommand = ReactiveCommand.Create(() => Close());
        }
        
        public override void Activate(NodesRemovingNavigationParameter parameter)
        {
            Files = parameter.Files;
            this.RaisePropertyChanged(nameof(FilesCount));
            this.RaisePropertyChanged(nameof(ShouldShowFilesList));
        }
    }
}