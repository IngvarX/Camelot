using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Behaviors;
using Camelot.Services.Abstractions.Operations;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.MainWindow.FilePanels
{
    public abstract class FileSystemNodeViewModelBase : ViewModelBase, IFileSystemNodeViewModel
    {
        private readonly IFileSystemNodeOpeningBehavior _fileSystemNodeOpeningBehavior;
        private readonly IOperationsService _operationsService;
        private readonly IClipboardOperationsService _clipboardOperationsService;
        private readonly IFilesOperationsMediator _filesOperationsMediator;
        private readonly IFileSystemNodePropertiesBehavior _fileSystemNodePropertiesBehavior;

        private DateTime _lastModifiedDateTime;
        private string _fullPath;
        private string _name;
        private string _fullName;
        private bool _isEditing;

        private IReadOnlyList<string> Files => new[] {FullPath};

        public DateTime LastModifiedDateTime
        {
            get => _lastModifiedDateTime;
            set => this.RaiseAndSetIfChanged(ref _lastModifiedDateTime, value);
        }

        public string FullPath
        {
            get => _fullPath;
            set => this.RaiseAndSetIfChanged(ref _fullPath, value);
        }

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public string FullName
        {
            get => _fullName;
            set => this.RaiseAndSetIfChanged(ref _fullName, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => this.RaiseAndSetIfChanged(ref _isEditing, value);
        }

        public bool IsWaitingForEdit { get; set; }

        public ICommand OpenCommand { get; }

        public ICommand StartRenamingCommand { get; }

        public ICommand RenameCommand { get; }

        public ICommand CopyToClipboardCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand CopyCommand { get; }

        public ICommand MoveCommand { get; }

        public ICommand ShowPropertiesCommand { get; }

        protected FileSystemNodeViewModelBase(
            IFileSystemNodeOpeningBehavior fileSystemNodeOpeningBehavior,
            IOperationsService operationsService,
            IClipboardOperationsService clipboardOperationsService,
            IFilesOperationsMediator filesOperationsMediator,
            IFileSystemNodePropertiesBehavior fileSystemNodePropertiesBehavior)
        {
            _fileSystemNodeOpeningBehavior = fileSystemNodeOpeningBehavior;
            _operationsService = operationsService;
            _clipboardOperationsService = clipboardOperationsService;
            _filesOperationsMediator = filesOperationsMediator;
            _fileSystemNodePropertiesBehavior = fileSystemNodePropertiesBehavior;

            OpenCommand = ReactiveCommand.Create(Open);
            StartRenamingCommand = ReactiveCommand.Create(StartRenaming);
            RenameCommand = ReactiveCommand.Create(Rename);
            CopyToClipboardCommand = ReactiveCommand.CreateFromTask(CopyToClipboardAsync);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync);
            CopyCommand = ReactiveCommand.CreateFromTask(CopyAsync);
            MoveCommand = ReactiveCommand.CreateFromTask(MoveAsync);
            ShowPropertiesCommand = ReactiveCommand.CreateFromTask(ShowPropertiesAsync);
        }

        private void Open() => _fileSystemNodeOpeningBehavior.Open(FullPath);

        private void StartRenaming() => IsEditing = true;

        private void Rename()
        {
            if (string.IsNullOrEmpty(_fullName))
            {
                return;
            }

            var renameResult = _operationsService.Rename(_fullPath, _fullName);
            if (renameResult)
            {
                IsEditing = false;
            }
        }

        private Task CopyToClipboardAsync() => _clipboardOperationsService.CopyFilesAsync(Files);

        private Task DeleteAsync() => _operationsService.RemoveAsync(Files);

        private Task CopyAsync() => _operationsService.CopyAsync(Files, _filesOperationsMediator.OutputDirectory);

        private Task MoveAsync() => _operationsService.MoveAsync(Files, _filesOperationsMediator.OutputDirectory);

        private Task ShowPropertiesAsync() => _fileSystemNodePropertiesBehavior.ShowPropertiesAsync(FullPath);
    }
}