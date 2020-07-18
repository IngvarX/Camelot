using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Interfaces.Properties;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties
{
    public class DirectoryInformationDialogViewModel : ParameterizedDialogViewModelBase<FileSystemNodeNavigationParameter>
    {
        private readonly IDirectoryService _directoryService;
        private readonly IApplicationDispatcher _applicationDispatcher;

        public IMainNodeInfoTabViewModel MainNodeInfoTabViewModel { get; }

        public DirectoryInformationDialogViewModel(
            IDirectoryService directoryService,
            IApplicationDispatcher applicationDispatcher,
            IMainNodeInfoTabViewModel mainNodeInfoTabViewModel)
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

        private void SetupMainTab(NodeModelBase directoryModel)
        {
            LoadDirectorySize(directoryModel.FullPath);

            MainNodeInfoTabViewModel.Activate(directoryModel, true);
        }

        private void LoadDirectorySize(string directory) =>
            Task
                .Factory
                .StartNew(() => _directoryService.CalculateSize(directory))
                .ContinueWith(t => SetSize(t.Result));

        private void SetSize(long size) =>
            _applicationDispatcher.Dispatch(() => MainNodeInfoTabViewModel.SetSize(size));
    }
}