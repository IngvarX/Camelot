using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties
{
    public class FileInformationDialogViewModel : ParameterizedDialogViewModelBase<FileSystemNodeNavigationParameter>
    {
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;

        public MainNodeInfoTabViewModel MainNodeInfoTabViewModel { get; }

        public FileInformationDialogViewModel(
            MainNodeInfoTabViewModel mainNodeInfoTabViewModel,
            IFileService fileService,
            IPathService pathService)
        {
            MainNodeInfoTabViewModel = mainNodeInfoTabViewModel;
            _fileService = fileService;
            _pathService = pathService;
        }

        public override void Activate(FileSystemNodeNavigationParameter parameter)
        {
            var fileModel = _fileService.GetFile(parameter.NodePath);

            SetupMainTab(fileModel);
        }

        private void SetupMainTab(FileModel fileModel)
        {
            MainNodeInfoTabViewModel.Name = fileModel.Name;
            MainNodeInfoTabViewModel.Path = _pathService.GetParentDirectory(fileModel.FullPath);
            MainNodeInfoTabViewModel.Size = fileModel.SizeBytes;
            MainNodeInfoTabViewModel.CreatedDateTime = fileModel.CreatedDateTime;
            MainNodeInfoTabViewModel.LastWriteDateTime = fileModel.LastModifiedDateTime;
            MainNodeInfoTabViewModel.LastAccessDateTime = fileModel.LastAccessDateTime;
        }
    }
}