using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Steps;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Flows.Dialogs;

public class OpenAboutDialogFlow : IDisposable
{
    private AboutDialog _dialog;

    [Fact(DisplayName = "Open about dialog")]
    public async Task TestAboutDialog()
    {
        var app = AvaloniaApp.GetApp();
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        OpenAboutDialogStep.OpenAboutDialog(window);
        await DialogOpenedCondition.CheckIfDialogIsOpenedAsync<AboutDialog>(app);

        _dialog = app
            .Windows
            .OfType<AboutDialog>()
            .Single();

        var githubButton = _dialog.GetVisualDescendants().OfType<Button>().SingleOrDefault();
        Assert.NotNull(githubButton);
        Assert.True(githubButton.IsDefault);
        Assert.True(githubButton.IsEnabled);
    }

    public void Dispose() => _dialog?.Close();
}