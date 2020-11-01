using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Archive;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Abstractions.Models.State;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Dialogs.Archives;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;
using Camelot.ViewModels.Services.Interfaces;
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
        private readonly ISystemDialogService _systemDialogService;
        private readonly ICreateArchiveStateService _createArchiveStateService;
        private readonly ObservableCollection<ArchiveTypeViewModel> _availableArchiveTypes;

        [Reactive]
        public string ArchivePath { get; set; }

        public IEnumerable<ArchiveTypeViewModel> AvailableArchiveTypes => _availableArchiveTypes;

        [Reactive]
        public ArchiveTypeViewModel SelectedArchiveType { get; set; }

        public ICommand SelectPathCommand { get; }

        public ICommand CreateCommand { get; }

        public ICommand CancelCommand { get; }

        public CreateArchiveDialogViewModel(
            IDirectoryService directoryService,
            IFileService fileService,
            IArchiveTypeViewModelFactory archiveTypeViewModelFactory,
            ISystemDialogService systemDialogService,
            ICreateArchiveStateService createArchiveStateService)
        {
            _directoryService = directoryService;
            _fileService = fileService;
            _archiveTypeViewModelFactory = archiveTypeViewModelFactory;
            _systemDialogService = systemDialogService;
            _createArchiveStateService = createArchiveStateService;
            _availableArchiveTypes = new ObservableCollection<ArchiveTypeViewModel>();

            this.WhenAnyValue(x => x.SelectedArchiveType)
                .Buffer(2, 1)
                .Select(b => (Previous: b[0], Current: b[1]))
                .Subscribe(values => ArchivePath = GetUpdatedArchivePath(values.Previous, values.Current));
            var canCreate = this.WhenAnyValue(x => x.ArchivePath,
                CheckIfPathIsValid);

            SelectPathCommand = ReactiveCommand.CreateFromTask(SelectPathAsync);
            CreateCommand = ReactiveCommand.Create(CreateArchive, canCreate);
            CancelCommand = ReactiveCommand.Create(Close);

            ArchivePath = string.Empty;
        }

        public override void Activate(CreateArchiveNavigationParameter parameter)
        {
            var archiveTypeViewModels = parameter.IsPackingSingleFile
                ? _archiveTypeViewModelFactory.CreateForSingleFile()
                : _archiveTypeViewModelFactory.CreateForMultipleFiles();
            _availableArchiveTypes.AddRange(archiveTypeViewModels);
            SelectedArchiveType = GetSelectedArchiveType();

            ArchivePath = $"{parameter.DefaultArchivePath}.{SelectedArchiveType.Name}";
        }

        private async Task SelectPathAsync()
        {
            var path = await _systemDialogService.GetFileAsync(ArchivePath);
            if (!string.IsNullOrWhiteSpace(path))
            {
                ArchivePath = path;
            }
        }

        private void CreateArchive()
        {
            SaveState();

            Close(new CreateArchiveDialogResult(ArchivePath, SelectedArchiveType.ArchiveType));
        }

        private void SaveState()
        {
            var state = new CreateArchiveStateModel
            {
                ArchiveType = SelectedArchiveType.ArchiveType
            };

            _createArchiveStateService.SaveState(state);
        }

        private bool CheckIfPathIsValid(string path) =>
            !string.IsNullOrWhiteSpace(path) &&
            !_fileService.CheckIfExists(path) &&
            !_directoryService.CheckIfExists(path);

        private string GetUpdatedArchivePath(ArchiveTypeViewModel previous, ArchiveTypeViewModel current)
        {
            var currentExtensionLength = previous?.Name?.Length ?? 0;
            var archivePathWithoutExtension = ArchivePath.Substring(0, ArchivePath.Length - currentExtensionLength);

            return $"{archivePathWithoutExtension}{current.Name}";
        }

        private ArchiveTypeViewModel GetSelectedArchiveType()
        {
            var stateModel = _createArchiveStateService.GetState();
            var archiveType = (ArchiveType) stateModel.ArchiveType;

            return _availableArchiveTypes.SingleOrDefault(vm => vm.ArchiveType == archiveType) ??
                   _availableArchiveTypes.First();
        }
    }
}