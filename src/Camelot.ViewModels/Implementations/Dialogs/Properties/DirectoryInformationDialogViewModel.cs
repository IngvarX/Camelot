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
        private readonly IApplicationDispatcher _applicationDispatcher;

        public MainNodeInfoTabViewModel MainNodeInfoTabViewModel { get; }

        public DirectoryInformationDialogViewModel(
            IDirectoryService directoryService,
            IApplicationDispatcher applicationDispatcher,
            MainNodeInfoTabViewModel mainNodeInfoTabViewModel)
        {
            _directoryService = directoryService;
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

            MainNodeInfoTabViewModel.FullPath = directoryModel.FullPath;
            MainNodeInfoTabViewModel.CreatedDateTime = directoryModel.CreatedDateTime;
            MainNodeInfoTabViewModel.LastWriteDateTime = directoryModel.LastModifiedDateTime;
            MainNodeInfoTabViewModel.LastAccessDateTime = directoryModel.LastAccessDateTime;
            MainNodeInfoTabViewModel.Type = NodeType.Directory;
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