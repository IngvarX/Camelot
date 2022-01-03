using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Extensions;
using Camelot.Ui.Tests.Steps;
using Xunit;

namespace Camelot.Ui.Tests.Flows;

public class UseDirectorySelectorFlow : IDisposable
{
    [Fact(DisplayName = "Go to parent directory and back using directory selector")]
    public async Task GoToParentDirectoryAndBackTest()
    {
        var window = AvaloniaApp.GetMainWindow();
        await FocusFilePanelStep.FocusFilePanelAsync(window);

        CreateNewTabStep.CreateNewTab(window);
        FocusDirectorySelectorStep.FocusDirectorySelector(window);

        var filesPanelViewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
        var currentDirectory = filesPanelViewModel.CurrentDirectory;
        var filesPanelView = ActiveFilePanelProvider.GetActiveFilePanelView(window);
        var directoryTextBox = filesPanelView
            .GetVisualDescendants()
            .OfType<TextBox>()
            .SingleOrDefault(t => t.Name == "DirectoryTextBox");
        Assert.NotNull(directoryTextBox);

        var separatorPosition = directoryTextBox.Text.LastIndexOf(Path.DirectorySeparatorChar);
        Assert.True(separatorPosition >= 0);

        directoryTextBox.CaretIndex = directoryTextBox.Text.Length;
        var symbolsToRemoveCount = directoryTextBox.Text.Length - separatorPosition;
        for (var i = 0; i < symbolsToRemoveCount; i++)
        {
            Keyboard.PressKey(window, Key.Back);
        }

        var isParentDirectoryOpened = await DirectoryOpenedCondition.CheckIfParentDirectoryIsOpenedAsync(window, currentDirectory);
        Assert.True(isParentDirectoryOpened);

        var directoryName = Path.GetFileNameWithoutExtension(currentDirectory);
        directoryTextBox.SendText(Path.DirectorySeparatorChar + directoryName);

        var childDirectoryWasOpened =
            await DirectoryOpenedCondition.CheckIfDirectoryIsOpenedAsync(window, currentDirectory);
        Assert.True(childDirectoryWasOpened);
    }

    public void Dispose()
    {
        var window = AvaloniaApp.GetMainWindow();
        CloseCurrentTabStep.CloseCurrentTab(window);
    }
}