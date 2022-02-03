using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Extensions;
using Camelot.Ui.Tests.Steps;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Flows.Dialogs;

public class OpenCloseCreateDirectoryDialogFlow : IDisposable
{
    private CreateDirectoryDialog _dialog;

    [Fact(DisplayName = "Open and close create directory dialog")]
    public async Task TestCreateDirectoryDialog()
    {
        var app = AvaloniaApp.GetApp();
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        OpenCreateDirectoryDialogStep.OpenCreateDirectoryDialog(window);
        var isDialogOpened = await DialogOpenedCondition.CheckIfDialogIsOpenedAsync<CreateDirectoryDialog>(app);
        Assert.True(isDialogOpened);

        _dialog = app
            .Windows
            .OfType<CreateDirectoryDialog>()
            .Single();

        var buttons = _dialog
            .GetVisualDescendants()
            .OfType<Button>()
            .ToArray();
        Assert.Equal(2, buttons.Length);
        var createButton = buttons.SingleOrDefault(b => !b.Classes.Contains("transparentDialogButton"));

        Assert.NotNull(createButton);
        Assert.False(createButton.Command.CanExecute(null));
        Assert.True(createButton.IsDefault);

        var directoryNameTextBox = _dialog
            .GetVisualDescendants()
            .OfType<TextBox>()
            .SingleOrDefault();
        Assert.NotNull(directoryNameTextBox);
        Assert.True(string.IsNullOrEmpty(directoryNameTextBox.Text));
        Assert.True(directoryNameTextBox.IsFocused);

        directoryNameTextBox.SendText("DirectoryName");

        await WaitService.WaitForConditionAsync(() => createButton.Command.CanExecute(null));

        var closeButton = buttons.SingleOrDefault(b => b.Classes.Contains("transparentDialogButton"));
        Assert.NotNull(closeButton);
        Assert.True(closeButton.Command.CanExecute(null));
        Assert.False(closeButton.IsDefault);

        closeButton.Command.Execute(null);

        var isDialogClosed = await DialogClosedCondition.CheckIfDialogIsClosedAsync<CreateDirectoryDialog>(app);
        Assert.True(isDialogClosed);
    }

    public void Dispose() => _dialog?.Close();
}