using System.IO;
using System.Threading.Tasks;
using Camelot.Ui.Tests.Common;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.Views;

namespace Camelot.Ui.Tests.Conditions;

public static class DirectoryOpenedCondition
{
    public static Task<bool> CheckIfParentDirectoryIsOpenedAsync(MainWindow mainWindow, string directory) =>
        CheckIfDirectoryIsOpenedAsync(mainWindow, Directory.GetParent(directory).FullName);

    public static async Task<bool> CheckIfDirectoryIsOpenedAsync(MainWindow mainWindow, string directory)
    {
        var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(mainWindow);

        return await WaitService.WaitForConditionAsync(() => CheckIfDirectoryWasOpened(viewModel, directory), 100, 50);
    }

    private static bool CheckIfDirectoryWasOpened(IFilesPanelViewModel viewModel, string directoryPath) =>
        viewModel.CurrentDirectory == directoryPath;
}