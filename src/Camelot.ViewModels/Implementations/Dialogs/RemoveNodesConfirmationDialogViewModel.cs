using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class RemoveNodesConfirmationDialogViewModel : ParameterizedDialogViewModelBase<RemoveNodesConfirmationDialogResult, NodesRemovingNavigationParameter>
    {
        private const int ShowedFilesLimit = 4;

        private readonly IPathService _pathService;

        private IEnumerable<string> _files;

        public IEnumerable<string> Files
        {
            get => _files;
            set
            {
                this.RaiseAndSetIfChanged(ref _files, value);
                this.RaisePropertyChanged(nameof(FilesCount));
                this.RaisePropertyChanged(nameof(ShouldShowFilesList));
            }
        }

        public int FilesCount => Files.Count();

        public bool ShouldShowFilesList => FilesCount <= ShowedFilesLimit;

        [Reactive]
        public bool IsRemovingToTrash { get; set; }

        public ICommand OkCommand { get; }

        public ICommand CancelCommand { get; }

        public RemoveNodesConfirmationDialogViewModel(
            IPathService pathService)
        {
            _pathService = pathService;
            _files = Enumerable.Empty<string>();

            OkCommand = ReactiveCommand.Create(Ok);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        public override void Activate(NodesRemovingNavigationParameter parameter)
        {
            Files = parameter.Files.Select(_pathService.GetFileName);
            IsRemovingToTrash = parameter.IsRemovingToTrash;
        }

        private void Ok() => Close(CreateResult(true));

        private void Cancel() => Close(CreateResult(false));

        private static RemoveNodesConfirmationDialogResult CreateResult(bool isConfirmed) =>
            new RemoveNodesConfirmationDialogResult(isConfirmed);
    }
}