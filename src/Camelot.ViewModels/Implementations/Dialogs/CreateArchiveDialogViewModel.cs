using System;
using System.Collections.Generic;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class CreateArchiveDialogViewModel : ParameterizedDialogViewModelBase<CreateArchiveDialogResult, CreateArchiveNavigationParameter>
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;

        [Reactive]
        public string ArchivePath { get; set; }

        public IEnumerable<ArchiveType> AvailableArchiveTypes => new[] {ArchiveType.Tar};

        [Reactive]
        public ArchiveType ArchiveType { get; set; }

        public ICommand CreateCommand { get; }

        public ICommand CancelCommand { get; }

        public CreateArchiveDialogViewModel(
            IDirectoryService directoryService,
            IFileService fileService)
        {
            _directoryService = directoryService;
            _fileService = fileService;

            var canCreate = this.WhenAnyValue(x => x.ArchivePath,
                CheckIfPathIsValid);

            CreateCommand = ReactiveCommand.Create(CreateArchive, canCreate);
            CancelCommand = ReactiveCommand.Create(Close);
        }

        public override void Activate(CreateArchiveNavigationParameter parameter) =>
            ArchivePath = parameter.DefaultArchivePath;

        private void CreateArchive() =>
            Close(new CreateArchiveDialogResult(ArchivePath, ArchiveType));

        private bool CheckIfPathIsValid(string path) =>
            !string.IsNullOrWhiteSpace(path) &&
            !_fileService.CheckIfExists(path) &&
            !_directoryService.CheckIfExists(path);
    }
}