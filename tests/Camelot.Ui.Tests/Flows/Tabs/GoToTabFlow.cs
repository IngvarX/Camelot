using System;
using System.Threading.Tasks;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Steps;
using Xunit;

namespace Camelot.Ui.Tests.Flows.Tabs;

public class GoToTabFlow : IDisposable
{
    private const int TabsCount = 10;

    [Fact(DisplayName = "Go to tab by index")]
    public async Task TestGoToTab()
    {
        var app = AvaloniaApp.GetApp();
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        for (var i = 0; i < TabsCount; i++)
        {
            CreateNewTabStep.CreateNewTab(window);
        }

        for (var i = 1; i < 10; i++)
        {
            GoToTabStep.GoToTab(window, i);
            await TabOpenedCondition.CheckIfTabIsOpenedAsync(app, i);
        }

        GoToTabStep.GoToLastTab(window);
        await TabOpenedCondition.CheckIfLastTabIsOpenedAsync(app);
    }

    public void Dispose()
    {
        var window = AvaloniaApp.GetMainWindow();

        for (var i = 0; i < TabsCount; i++)
        {
            CloseCurrentTabStep.CloseCurrentTab(window);
        }
    }
}