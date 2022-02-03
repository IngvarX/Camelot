using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Interfaces.Properties;

namespace Camelot.ViewModels.Implementations.Dialogs.Properties;

public class FileInformationDialogViewModel : ParameterizedDialogViewModelBase<FileSystemNodeNavigationParameter>
{
    private readonly IFileService _fileService;

    public IMainNodeInfoTabViewModel MainNodeInfoTabViewModel { get; }

    public FileInformationDialogViewModel(
        IFileService fileService,
        IMainNodeInfoTabViewModel mainNodeInfoTabViewModel)
    {
        _fileService = fileService;

        MainNodeInfoTabViewModel = mainNodeInfoTabViewModel;
    }

    public override void Activate(FileSystemNodeNavigationParameter parameter)
    {
        var fileModel = _fileService.GetFile(parameter.NodePath);

        SetupMainTab(fileModel);
    }

    private void SetupMainTab(FileModel fileModel)
    {
        MainNodeInfoTabViewModel.Activate(fileModel);
        MainNodeInfoTabViewModel.SetSize(fileModel.SizeBytes);
    }
}