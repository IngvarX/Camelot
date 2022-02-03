using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Camelot.Ui.Tests.Common;

namespace Camelot.Ui.Tests.Conditions;

public static class DialogOpenedCondition
{
    public static Task<bool> CheckIfDialogIsOpenedAsync<TDialog>(IClassicDesktopStyleApplicationLifetime app) =>
        WaitService.WaitForConditionAsync(() => CheckIfDialogIsOpened<TDialog>(app));

    private static bool CheckIfDialogIsOpened<TDialog>(IClassicDesktopStyleApplicationLifetime app) =>
        app
            .Windows
            .OfType<TDialog>()
            .Any();
}