using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Interfaces.Properties;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties;

public class DirectoryInformationDialogViewModel : ParameterizedDialogViewModelBase<FileSystemNodeNavigationParameter>
{
    private readonly IDirectoryService _directoryService;
    private readonly IFileService _fileService;
    private readonly IApplicationDispatcher _applicationDispatcher;

    public IMainNodeInfoTabViewModel MainNodeInfoTabViewModel { get; }

    public DirectoryInformationDialogViewModel(
        IDirectoryService directoryService,
        IFileService fileService,
        IApplicationDispatcher applicationDispatcher,
        IMainNodeInfoTabViewModel mainNodeInfoTabViewModel)
    {
        _directoryService = directoryService;
        _fileService = fileService;
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

        var filesCount = _fileService.GetFiles(directoryModel.FullPath).Count;
        var directoriesCount = _directoryService.GetChildDirectories(directoryModel.FullPath).Count;

        MainNodeInfoTabViewModel.Activate(directoryModel, true, filesCount, directoriesCount);
    }

    private void LoadDirectorySize(string directory) =>
        Task
            .Run(() => _directoryService.CalculateSize(directory))
            .ContinueWith(t => SetSize(t.Result));

    private void SetSize(long size) =>
        _applicationDispatcher.Dispatch(() => MainNodeInfoTabViewModel.SetSize(size));
}