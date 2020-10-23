using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs.Archives;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class CreateArchiveDialogViewModel : ParameterizedDialogViewModelBase<CreateArchiveDialogResult, CreateArchiveNavigationParameter>
    {
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;
        private readonly IArchiveTypeViewModelFactory _archiveTypeViewModelFactory;
        private readonly ObservableCollection<ArchiveTypeViewModel> _availableArchiveTypes;

        [Reactive]
        public string ArchivePath { get; set; }

        public IEnumerable<ArchiveTypeViewModel> AvailableArchiveTypes => _availableArchiveTypes;

        [Reactive]
        public ArchiveTypeViewModel SelectedArchiveType { get; set; }

        public ICommand CreateCommand { get; }

        public ICommand CancelCommand { get; }

        public CreateArchiveDialogViewModel(
            IDirectoryService directoryService,
            IFileService fileService,
            IArchiveTypeViewModelFactory archiveTypeViewModelFactory)
        {
            _directoryService = directoryService;
            _fileService = fileService;
            _archiveTypeViewModelFactory = archiveTypeViewModelFactory;
            _availableArchiveTypes = new ObservableCollection<ArchiveTypeViewModel>();

            var canCreate = this.WhenAnyValue(x => x.ArchivePath,
                CheckIfPathIsValid);

            CreateCommand = ReactiveCommand.Create(CreateArchive, canCreate);
            CancelCommand = ReactiveCommand.Create(Close);
        }

        public override void Activate(CreateArchiveNavigationParameter parameter)
        {
            ArchivePath = parameter.DefaultArchivePath;
            var archiveTypeViewModels = parameter.IsPackingSingleFile
                ? _archiveTypeViewModelFactory.CreateForSingleFile()
                : _archiveTypeViewModelFactory.CreateForMultipleFiles();
            _availableArchiveTypes.AddRange(archiveTypeViewModels);
            SelectedArchiveType = archiveTypeViewModels.First();
        }

        private void CreateArchive() =>
            Close(new CreateArchiveDialogResult(ArchivePath, SelectedArchiveType.ArchiveType));

        private bool CheckIfPathIsValid(string path) =>
            !string.IsNullOrWhiteSpace(path) &&
            !_fileService.CheckIfExists(path) &&
            !_directoryService.CheckIfExists(path);
    }
}