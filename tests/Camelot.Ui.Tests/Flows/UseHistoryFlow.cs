using System;
using System.Threading.Tasks;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Steps;
using Xunit;

namespace Camelot.Ui.Tests.Flows;

public class UseHistoryFlow : IDisposable
{
    private string _directoryFullPath;

    [Fact(DisplayName = "Go to parent directory and back using history")]
    public async Task GoToParentDirectoryAndBackTest()
    {
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);
        CreateNewTabStep.CreateNewTab(window);
        var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);

        _directoryFullPath = viewModel.CurrentDirectory;

        for (var i = 0; i < 10; i++)
        {
            GoToPreviousDirectoryStep.GoToPreviousDirectory(window);
            var isCurrentDirectoryStillOpened = await DirectoryOpenedCondition.CheckIfDirectoryIsOpenedAsync(window, _directoryFullPath);
            Assert.True(isCurrentDirectoryStillOpened);
        }

        GoToParentDirectoryStep.GoToParentDirectoryViaFilePanel(window);
        var isParentDirectoryOpened = await DirectoryOpenedCondition.CheckIfParentDirectoryIsOpenedAsync(window, _directoryFullPath);
        Assert.True(isParentDirectoryOpened);

        GoToPreviousDirectoryStep.GoToPreviousDirectory(window);
        var isChildDirectoryOpened = await DirectoryOpenedCondition.CheckIfDirectoryIsOpenedAsync(window, _directoryFullPath);
        Assert.True(isChildDirectoryOpened);

        for (var i = 0; i < 10; i++)
        {
            GoToNextDirectoryStep.GoToNextDirectory(window);
            var parentDirectoryWasReopened = await DirectoryOpenedCondition.CheckIfParentDirectoryIsOpenedAsync(window,
                _directoryFullPath);
            Assert.True(parentDirectoryWasReopened);
        }

        for (var i = 0; i < 10; i++)
        {
            GoToPreviousDirectoryStep.GoToPreviousDirectory(window);
            var isCurrentDirectoryStillOpened = await DirectoryOpenedCondition.CheckIfDirectoryIsOpenedAsync(window, _directoryFullPath);
            Assert.True(isCurrentDirectoryStillOpened);
        }
    }

    public void Dispose()
    {
        if (_directoryFullPath is null)
        {
            return;
        }

        var window = AvaloniaApp.GetMainWindow();
        CloseCurrentTabStep.CloseCurrentTab(window);
    }
}