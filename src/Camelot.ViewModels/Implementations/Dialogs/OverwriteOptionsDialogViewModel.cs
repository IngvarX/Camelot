using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.Operations;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class OverwriteOptionsDialogViewModel : ParameterizedDialogViewModelBase<OverwriteOptionsDialogResult, OverwriteOptionsNavigationParameter>
    {
        private readonly IFileService _fileService;
        private readonly IFileSystemNodeViewModelFactory _fileSystemNodeViewModelFactory;
        private readonly IFileNameGenerationService _fileNameGenerationService;
        private readonly IPathService _pathService;

        private IFileSystemNodeViewModel _sourceFileViewModel;
        private IFileSystemNodeViewModel _destinationFileViewModel;
        private bool _shouldApplyForAll;
        private string _newFileName;
        private string _destinationDirectoryName;
        private bool _areMultipleFilesAvailable;

        public bool ShouldApplyForAll
        {
            get => _shouldApplyForAll;
            set => this.RaiseAndSetIfChanged(ref _shouldApplyForAll, value);
        }

        public string NewFileName
        {
            get => _newFileName;
            set => this.RaiseAndSetIfChanged(ref _newFileName, value);
        }

        public string DestinationDirectoryName
        {
            get => _destinationDirectoryName;
            set => this.RaiseAndSetIfChanged(ref _destinationDirectoryName, value);
        }

        public IFileSystemNodeViewModel SourceFileViewModel
        {
            get => _sourceFileViewModel;
            set => this.RaiseAndSetIfChanged(ref _sourceFileViewModel, value);
        }

        public IFileSystemNodeViewModel DestinationFileViewModel
        {
            get => _sourceFileViewModel;
            set => this.RaiseAndSetIfChanged(ref _destinationFileViewModel, value);
        }

        public bool AreMultipleFilesAvailable
        {
            get => _areMultipleFilesAvailable;
            set => this.RaiseAndSetIfChanged(ref _areMultipleFilesAvailable, value);
        }

        public ICommand CancelCommand { get; }

        public ICommand SkipCommand { get; }

        public ICommand ReplaceCommand { get; }

        public ICommand ReplaceIfOlderCommand { get; }

        public ICommand RenameCommand { get; }

        public OverwriteOptionsDialogViewModel(
            IFileService fileService,
            IFileSystemNodeViewModelFactory fileSystemNodeViewModelFactory,
            IFileNameGenerationService fileNameGenerationService,
            IPathService pathService)
        {
            _fileService = fileService;
            _fileSystemNodeViewModelFactory = fileSystemNodeViewModelFactory;
            _fileNameGenerationService = fileNameGenerationService;
            _pathService = pathService;

            CancelCommand = ReactiveCommand.Create(() => Close());
            SkipCommand = ReactiveCommand.Create(Skip);
            ReplaceCommand = ReactiveCommand.Create(Replace);
            ReplaceIfOlderCommand = ReactiveCommand.Create(ReplaceIfOlder);
            RenameCommand = ReactiveCommand.Create(Rename);
        }

        public override void Activate(OverwriteOptionsNavigationParameter parameter)
        {
            var sourceFileModel = _fileService.GetFile(parameter.SourceFilePath);
            SourceFileViewModel = _fileSystemNodeViewModelFactory.Create(sourceFileModel);

            var destinationFileModel = _fileService.GetFile(parameter.DestinationFilePath);
            DestinationFileViewModel = _fileSystemNodeViewModelFactory.Create(destinationFileModel);

            var destinationDirectory = _pathService.GetParentDirectory(destinationFileModel.FullPath);
            NewFileName = _fileNameGenerationService.GenerateName(sourceFileModel.Name, destinationDirectory);
            DestinationDirectoryName = _pathService.GetFileName(destinationDirectory);

            AreMultipleFilesAvailable = parameter.AreMultipleFilesAvailable;
        }

        private void Skip() => Close(Create(OperationContinuationMode.Skip));

        private void Replace() => Close(Create(OperationContinuationMode.Overwrite));

        private void ReplaceIfOlder() => Close(Create(OperationContinuationMode.OverwriteOlder));

        private void Rename()
        {
            var destinationDirectory = _pathService.GetParentDirectory(DestinationFileViewModel.FullPath);
            var destinationFilePath = _pathService.Combine(destinationDirectory, _newFileName);
            var options = OperationContinuationOptions.CreateRenamingContinuationOptions(
                SourceFileViewModel.FullPath,
                _shouldApplyForAll,
                destinationFilePath
            );

            Close(options);
        }

        private void Close(OperationContinuationOptions options) =>
            Close(new OverwriteOptionsDialogResult(options));

        private OperationContinuationOptions Create(OperationContinuationMode mode) =>
            OperationContinuationOptions.CreateContinuationOptions(
                SourceFileViewModel.FullPath,
                _shouldApplyForAll,
                mode
            );
    }
}