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

public class OpenCloseSettingsDialogFlow : IDisposable
{
    private SettingsDialog _dialog;

    [Fact(DisplayName = "Open and close settings dialog")]
    public async Task TestSettingsDialog()
    {
        var app = AvaloniaApp.GetApp();
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);
        OpenSettingsDialogStep.OpenSettingsDialog(window);
        await DialogOpenedCondition.CheckIfDialogIsOpenedAsync<SettingsDialog>(app);

        _dialog = app
            .Windows
            .OfType<SettingsDialog>()
            .Single();
        var closeButton = _dialog
            .GetVisualDescendants()
            .OfType<Button>()
            .SingleOrDefault(b => b.Classes.Contains("transparentDialogButton"));
        Assert.NotNull(closeButton);

        Assert.True(closeButton.Command.CanExecute(null));
        closeButton.Command.Execute(null);

        var isClosed = await DialogClosedCondition.CheckIfDialogIsClosedAsync<SettingsDialog>(app);
        Assert.True(isClosed);
    }

    public void Dispose() => _dialog?.Close();
}