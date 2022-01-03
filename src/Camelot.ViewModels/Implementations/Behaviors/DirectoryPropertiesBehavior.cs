using System.Threading.Tasks;
using Camelot.ViewModels.Implementations.Dialogs.NavigationParameters;
using Camelot.ViewModels.Implementations.Dialogs.Properties;
using Camelot.ViewModels.Interfaces.Behaviors;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Implementations.Behaviors;

public class DirectoryPropertiesBehavior : IFileSystemNodePropertiesBehavior
{
    private readonly IDialogService _dialogService;

    public DirectoryPropertiesBehavior(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task ShowPropertiesAsync(string directoryPath)
    {
        var parameter = new FileSystemNodeNavigationParameter(directoryPath);

        await _dialogService.ShowDialogAsync(nameof(DirectoryInformationDialogViewModel), parameter);
    }
}