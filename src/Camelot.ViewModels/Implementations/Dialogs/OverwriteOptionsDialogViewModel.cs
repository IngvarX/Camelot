using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
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

        private IFileSystemNodeViewModel _replaceWithFileViewModel;
        private IFileSystemNodeViewModel _originalFileViewModel;
        private bool _shouldApplyToAll;
        private string _newFileName;
        private string _destinationDirectoryName;
        private bool _areMultipleFilesAvailable;

        public bool ShouldApplyToAll
        {
            get => _shouldApplyToAll;
            set => this.RaiseAndSetIfChanged(ref _shouldApplyToAll, value);
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

        public IFileSystemNodeViewModel ReplaceWithFileViewModel
        {
            get => _replaceWithFileViewModel;
            set => this.RaiseAndSetIfChanged(ref _replaceWithFileViewModel, value);
        }

        public IFileSystemNodeViewModel OriginalFileViewModel
        {
            get => _originalFileViewModel;
            set => this.RaiseAndSetIfChanged(ref _originalFileViewModel, value);
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

            CancelCommand = ReactiveCommand.Create(Close);
            SkipCommand = ReactiveCommand.Create(Skip);
            ReplaceCommand = ReactiveCommand.Create(Replace);
            ReplaceIfOlderCommand = ReactiveCommand.Create(ReplaceIfOlder);
            RenameCommand = ReactiveCommand.Create(Rename);
        }

        public override void Activate(OverwriteOptionsNavigationParameter parameter)
        {
            var sourceFileModel = _fileService.GetFile(parameter.SourceFilePath);
            ReplaceWithFileViewModel = CreateFrom(sourceFileModel);

            var destinationFileModel = _fileService.GetFile(parameter.DestinationFilePath);
            OriginalFileViewModel = CreateFrom(destinationFileModel);

            var destinationDirectory = _pathService.GetParentDirectory(destinationFileModel.FullPath);
            NewFileName = _fileNameGenerationService.GenerateName(sourceFileModel.Name, destinationDirectory);
            DestinationDirectoryName = _pathService.GetFileName(destinationDirectory);

            AreMultipleFilesAvailable = parameter.AreMultipleFilesAvailable;
        }

        private void Skip() => Close(CreateFrom(OperationContinuationMode.Skip));

        private void Replace() => Close(CreateFrom(OperationContinuationMode.Overwrite));

        private void ReplaceIfOlder() => Close(CreateFrom(OperationContinuationMode.OverwriteIfOlder));

        private void Rename()
        {
            var destinationDirectory = _pathService.GetParentDirectory(OriginalFileViewModel.FullPath);
            var destinationFilePath = _pathService.Combine(destinationDirectory, _newFileName);
            var options = OperationContinuationOptions.CreateRenamingContinuationOptions(
                ReplaceWithFileViewModel.FullPath,
                _shouldApplyToAll,
                destinationFilePath
            );

            Close(options);
        }

        private void Close(OperationContinuationOptions options) =>
            Close(new OverwriteOptionsDialogResult(options));

        private OperationContinuationOptions CreateFrom(OperationContinuationMode mode) =>
            OperationContinuationOptions.CreateContinuationOptions(
                ReplaceWithFileViewModel.FullPath,
                _shouldApplyToAll,
                mode
            );

        private IFileSystemNodeViewModel CreateFrom(FileModel fileModel) =>
            _fileSystemNodeViewModelFactory.Create(fileModel);
    }
}