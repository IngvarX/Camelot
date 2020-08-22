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
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class OverwriteOptionsDialogViewModel : ParameterizedDialogViewModelBase<OverwriteOptionsDialogResult, OverwriteOptionsNavigationParameter>
    {
        private readonly IFileService _fileService;
        private readonly IFileSystemNodeViewModelFactory _fileSystemNodeViewModelFactory;
        private readonly IFileNameGenerationService _fileNameGenerationService;
        private readonly IPathService _pathService;

        [Reactive]
        public bool ShouldApplyToAll { get; set; }

        [Reactive]
        public string NewFileName { get; set; }

        [Reactive]
        public string DestinationDirectoryName { get; set; }

        [Reactive]
        public IFileSystemNodeViewModel ReplaceWithFileViewModel { get; set; }

        [Reactive]
        public IFileSystemNodeViewModel OriginalFileViewModel { get; set; }

        [Reactive]
        public bool AreMultipleFilesAvailable { get; set; }

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
            var destinationFilePath = _pathService.Combine(destinationDirectory, NewFileName);
            var options = OperationContinuationOptions.CreateRenamingContinuationOptions(
                ReplaceWithFileViewModel.FullPath,
                ShouldApplyToAll,
                destinationFilePath
            );

            Close(options);
        }

        private void Close(OperationContinuationOptions options) =>
            Close(new OverwriteOptionsDialogResult(options));

        private OperationContinuationOptions CreateFrom(OperationContinuationMode mode) =>
            OperationContinuationOptions.CreateContinuationOptions(
                ReplaceWithFileViewModel.FullPath,
                ShouldApplyToAll,
                mode
            );

        private IFileSystemNodeViewModel CreateFrom(FileModel fileModel) =>
            _fileSystemNodeViewModelFactory.Create(fileModel);
    }
}