using System;
using System.Threading.Tasks;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Steps;
using Xunit;

namespace Camelot.Ui.Tests.Flows;

public class GoToParentDirectoryFlow : IDisposable
{
    [Fact(DisplayName = "Go to parent directory using file panel")]
    public async Task GoToParentDirectoryTest()
    {
        var window = AvaloniaApp.GetMainWindow();
        await FocusFilePanelStep.FocusFilePanelAsync(window);

        CreateNewTabStep.CreateNewTab(window);
        var filesPanelViewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
        var currentDirectory = filesPanelViewModel.CurrentDirectory;
        GoToParentDirectoryStep.GoToParentDirectoryViaFilePanel(window);

        var isParentDirectoryOpened = await DirectoryOpenedCondition.CheckIfParentDirectoryIsOpenedAsync(window, currentDirectory);
        Assert.True(isParentDirectoryOpened);
    }

    public void Dispose()
    {
        var window = AvaloniaApp.GetMainWindow();
        CloseCurrentTabStep.CloseCurrentTab(window);
    }
}