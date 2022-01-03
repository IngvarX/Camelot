using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Tabs;
using Camelot.Views.Main.Controls.Tabs;
using DynamicData;

namespace Camelot.Ui.Tests.Conditions;

public static class TabOpenedCondition
{
    public static Task<bool> CheckIfTabIsOpenedAsync(
        IClassicDesktopStyleApplicationLifetime app,
        int index) =>
        WaitService.WaitForConditionAsync(() => CheckIfTabIsOpened(app, index));

    public static Task<bool> CheckIfLastTabIsOpenedAsync(
        IClassicDesktopStyleApplicationLifetime app) =>
        WaitService.WaitForConditionAsync(() => CheckIfTabIsOpened(app, -1));

    private static bool CheckIfTabIsOpened(
        IClassicDesktopStyleApplicationLifetime app,
        int index)
    {
        var tabs = app.MainWindow
            .GetVisualDescendants()
            .OfType<TabView>()
            .ToArray();
        var selectedTab = tabs.SingleOrDefault(IsSelected);
        var expectedIndex = (index + tabs.Length - 1) % tabs.Length;

        return selectedTab is not null && tabs.IndexOf(selectedTab) == expectedIndex;
    }

    private static bool IsSelected(IDataContextProvider tabView)
    {
        var dataContext = (TabViewModel) tabView.DataContext;

        return dataContext?.IsGloballyActive ?? false;
    }
}