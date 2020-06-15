using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties
{
    public class DirectoryInformationDialogViewModel : FileSystemNodeInformationDialogViewModelBase
    {
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly IApplicationDispatcher _applicationDispatcher;

        public DirectoryInformationDialogViewModel(
            IFileSizeFormatter fileSizeFormatter,
            IDirectoryService directoryService,
            IPathService pathService,
            IApplicationDispatcher applicationDispatcher)
            : base(fileSizeFormatter)
        {
            _directoryService = directoryService;
            _pathService = pathService;
            _applicationDispatcher = applicationDispatcher;
        }

        public override void Activate(FileSystemNodeNavigationParameter parameter)
        {
            var directoryModel = _directoryService.GetDirectory(parameter.NodePath);

            Name = directoryModel.Name;
            Path = _pathService.GetParentDirectory(directoryModel.FullPath);
            CreatedDateTime = directoryModel.CreatedDateTime;
            LastWriteDateTime = directoryModel.LastModifiedDateTime;
            LastAccessDateTime = directoryModel.LastAccessDateTime;

            LoadDirectorySize(directoryModel.FullPath);
        }

        private void LoadDirectorySize(string directory)
        {
            Task.Factory
                .StartNew(() => _directoryService.CalculateSize(directory))
                .ContinueWith(t => SetSize(t.Result));
        }

        private void SetSize(long size) => _applicationDispatcher.Dispatch(() => Size = size);
    }
}