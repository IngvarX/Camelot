using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties
{
    public class FileInformationDialogViewModel : FileSystemNodeInformationDialogViewModelBase
    {
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;

        public FileInformationDialogViewModel(
            IFileSizeFormatter fileSizeFormatter,
            IFileService fileService,
            IPathService pathService)
            : base(fileSizeFormatter)
        {
            _fileService = fileService;
            _pathService = pathService;
        }

        public override void Activate(FileSystemNodeNavigationParameter parameter)
        {
            var fileModel = _fileService.GetFile(parameter.NodePath);

            Name = fileModel.Name;
            Path = _pathService.GetParentDirectory(fileModel.FullPath);
            Size = fileModel.SizeBytes;
            CreatedDateTime = fileModel.CreatedDateTime;
            LastWriteDateTime = fileModel.LastModifiedDateTime;
            LastAccessDateTime = fileModel.LastAccessDateTime;
        }
    }
}