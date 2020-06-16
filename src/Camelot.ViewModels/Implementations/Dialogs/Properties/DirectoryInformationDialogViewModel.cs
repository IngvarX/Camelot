using System.Threading.Tasks;
using ApplicationDispatcher.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties
{
    public class DirectoryInformationDialogViewModel : ParameterizedDialogViewModelBase<FileSystemNodeNavigationParameter>
    {
        private readonly IDirectoryService _directoryService;
        private readonly IPathService _pathService;
        private readonly IApplicationDispatcher _applicationDispatcher;

        public MainNodeInfoTabViewModel MainNodeInfoTabViewModel { get; }

        public DirectoryInformationDialogViewModel(
            IDirectoryService directoryService,
            IPathService pathService,
            IApplicationDispatcher applicationDispatcher,
            MainNodeInfoTabViewModel mainNodeInfoTabViewModel)
        {
            _directoryService = directoryService;
            _pathService = pathService;
            _applicationDispatcher = applicationDispatcher;

            MainNodeInfoTabViewModel = mainNodeInfoTabViewModel;
        }

        public override void Activate(FileSystemNodeNavigationParameter parameter)
        {
            var directoryModel = _directoryService.GetDirectory(parameter.NodePath);

            SetupMainTab(directoryModel);
        }

        private void SetupMainTab(DirectoryModel directoryModel)
        {
            LoadDirectorySize(directoryModel.FullPath);

            MainNodeInfoTabViewModel.Name = directoryModel.Name;
            MainNodeInfoTabViewModel.Path = _pathService.GetParentDirectory(directoryModel.FullPath);
            MainNodeInfoTabViewModel.CreatedDateTime = directoryModel.CreatedDateTime;
            MainNodeInfoTabViewModel.LastWriteDateTime = directoryModel.LastModifiedDateTime;
            MainNodeInfoTabViewModel.LastAccessDateTime = directoryModel.LastAccessDateTime;
        }

        private void LoadDirectorySize(string directory)
        {
            Task.Factory
                .StartNew(() => _directoryService.CalculateSize(directory))
                .ContinueWith(t => SetSize(t.Result));
        }

        private void SetSize(long size) => _applicationDispatcher.Dispatch(() => MainNodeInfoTabViewModel.Size = size);
    }
}