using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Camelot.Extensions;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Steps;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Flows.Files;

public class RemoveFileFlow : IDisposable
{
    private const string FileName = "RemoveFileTest__File.txt";

    private string _fileFullPath;

    [Theory(DisplayName = "Remove file")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestRemoveFile(bool removePermanently)
    {
        var app = AvaloniaApp.GetApp();
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
        _fileFullPath = Path.Combine(viewModel.CurrentDirectory, FileName);
        await File.Create(_fileFullPath).DisposeAsync();

        ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);

        await Task.Delay(100);

        SearchNodeStep.SearchNode(window, FileName);
        await Task.Delay(1000);

        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        Keyboard.PressKey(window, Key.Down);
        Keyboard.PressKey(window, Key.Down);

        if (removePermanently)
        {
            OpenRemoveDialogStep.OpenPermanentRemoveDialog(window);
        }
        else
        {
            OpenRemoveDialogStep.OpenRemoveDialog(window);
        }

        var isRemoveDialogOpened =
            await DialogOpenedCondition.CheckIfDialogIsOpenedAsync<RemoveNodesConfirmationDialog>(app);
        Assert.True(isRemoveDialogOpened);

        Keyboard.PressKey(window, Key.Enter);
        await Task.Delay(100);

        var isRemoveDialogClosed =
            await DialogClosedCondition.CheckIfDialogIsClosedAsync<RemoveNodesConfirmationDialog>(app);
        Assert.True(isRemoveDialogClosed);

        ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);

        Assert.False(File.Exists(_fileFullPath));
    }

    public void Dispose()
    {
        var app = AvaloniaApp.GetApp();
        var dialogs = new Window[]
        {
            DialogProvider.GetDialog<RemoveNodesConfirmationDialog>(app)
        };
        dialogs.ForEach(d => d?.Close());

        if (!string.IsNullOrEmpty(_fileFullPath) && File.Exists(_fileFullPath))
        {
            File.Delete(_fileFullPath);
        }
    }
}