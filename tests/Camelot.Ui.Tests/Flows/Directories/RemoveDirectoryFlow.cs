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

namespace Camelot.Ui.Tests.Flows.Directories;

public class RemoveDirectoryFlow : IDisposable
{
    private const string DirectoryName = "RemoveDirectoryTest__Directory";

    private string _directoryFullPath;

    [Theory(DisplayName = "Remove directory")]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestRemoveDirectory(bool removePermanently)
    {
        var app = AvaloniaApp.GetApp();
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
        _directoryFullPath = Path.Combine(viewModel.CurrentDirectory, DirectoryName);
        Directory.CreateDirectory(_directoryFullPath);

        ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);

        await Task.Delay(100);

        var filesPanel = ActiveFilePanelProvider.GetActiveFilePanelView(window);
        Assert.NotNull(filesPanel);

        SearchNodeStep.SearchNode(window, DirectoryName);

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

        Assert.False(Directory.Exists(_directoryFullPath));
    }

    public void Dispose()
    {
        var app = AvaloniaApp.GetApp();
        var dialogs = new Window[]
        {
            DialogProvider.GetDialog<RemoveNodesConfirmationDialog>(app)
        };
        dialogs.ForEach(d => d?.Close());

        if (!string.IsNullOrEmpty(_directoryFullPath) && Directory.Exists(_directoryFullPath))
        {
            Directory.Delete(_directoryFullPath, true);
        }
    }
}