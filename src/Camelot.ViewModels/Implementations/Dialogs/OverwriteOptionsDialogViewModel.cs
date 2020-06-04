using System.Windows.Input;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Results;

namespace Camelot.ViewModels.Implementations.Dialogs
{
    public class OverwriteOptionsDialogViewModel : ParameterizedDialogViewModelBase<OverwriteOptionsDialogResult, OverwriteOptionsNavigationParameter>
    {
        private readonly IFileService _fileService;

        private FileModel _sourceFileModel;
        private FileModel _destinationFileModel;

        public ICommand CancelCommand { get; }

        public ICommand SkipCommand { get; }

        public ICommand ReplaceCommand { get; }

        public OverwriteOptionsDialogViewModel(
            IFileService fileService)
        {
            _fileService = fileService;
        }

        public override void Activate(OverwriteOptionsNavigationParameter parameter)
        {
            _sourceFileModel = _fileService.GetFile(parameter.SourceFilePath);
            _destinationFileModel = _fileService.GetFile(parameter.DestinationFilePath);
        }
    }
}