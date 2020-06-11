using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class CreateDirectoryDialogViewModel : ParameterizedDialogViewModelBase<CreateDirectoryDialogResult, CreateDirectoryNavigationParameter>
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;

        private string _directoryName;
        private string _directoryPath;

        public string DirectoryName
        {
            get => _directoryName;
            set => this.RaiseAndSetIfChanged(ref _directoryName, value);
        }

        public ICommand CreateCommand { get; }

        public ICommand CancelCommand { get; }

        public CreateDirectoryDialogViewModel(
            IDirectoryService directoryService,
            IFileService fileService,
            IPathService pathService)
        {
            _directoryService = directoryService;
            _fileService = fileService;
            _pathService = pathService;

            var canCreate = this.WhenAnyValue(x => x.DirectoryName,
                IsNameValid);

            CreateCommand = ReactiveCommand.Create(CreateDirectory, canCreate);
            CancelCommand = ReactiveCommand.Create(Close);
        }

        public override void Activate(CreateDirectoryNavigationParameter navigationParameter)
        {
            _directoryPath = navigationParameter.DirectoryPath;
        }

        private void CreateDirectory()
        {
            var result = new CreateDirectoryDialogResult(_directoryName);

            Close(result);
        }

        private bool IsNameValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            var newFullPath = _pathService.Combine(_directoryPath, name);

            return !_fileService.CheckIfExists(newFullPath) && !_directoryService.CheckIfExists(newFullPath);
        }
    }
}