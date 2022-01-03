using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Camelot.Ui.Tests.Common;

namespace Camelot.Ui.Tests.Conditions;

public static class DialogClosedCondition
{
    public static Task<bool> CheckIfDialogIsClosedAsync<TDialog>(IClassicDesktopStyleApplicationLifetime app) =>
        WaitService.WaitForConditionAsync(() => CheckIfDialogIsClosed<TDialog>(app));

    private static bool CheckIfDialogIsClosed<TDialog>(IClassicDesktopStyleApplicationLifetime app) =>
        !app
            .Windows
            .OfType<TDialog>()
            .Any();
}