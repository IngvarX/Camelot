using System.Linq;
using System.Threading.Tasks;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Steps;
using Camelot.Views.Main.Controls.Tabs;
using Xunit;

namespace Camelot.Ui.Tests.Flows.Tabs;

public class CreateAndCloseNewTabFlow
{
    [Fact(DisplayName = "Create and close tab")]
    public async Task CreateAndCloseTabTest()
    {
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        var initialCount = GetTabsCount(window);

        for (var i = 0; i < 2; i++)
        {
            CreateNewTabStep.CreateNewTab(window);
            var isNewTabOpened = await WaitService.WaitForConditionAsync(() => initialCount + 1 == GetTabsCount(window));
            Assert.True(isNewTabOpened);

            CloseCurrentTabStep.CloseCurrentTab(window);
            var isTabClosed = await WaitService.WaitForConditionAsync(() => initialCount == GetTabsCount(window));
            Assert.True(isTabClosed);

            ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        }

        ReopenClosedTabStep.ReopenClosedTab(window);
        var isTabReopened = await WaitService.WaitForConditionAsync(() => initialCount + 1 == GetTabsCount(window));
        Assert.True(isTabReopened);

        CloseCurrentTabStep.CloseCurrentTab(window);
    }

    private static int GetTabsCount(IVisual window) =>
        window
            .GetVisualDescendants()
            .OfType<TabView>()
            .Count();
}