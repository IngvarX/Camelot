using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Extensions;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class SetDirectoryTextStep
{
    public static bool SetDirectoryText(MainWindow window, string text)
    {
        var filesPanelView = ActiveFilePanelProvider.GetActiveFilePanelView(window);
        var directoryTextBox = filesPanelView
            .GetVisualDescendants()
            .OfType<TextBox>()
            .SingleOrDefault(t => t.Name == "DirectoryTextBox");
        if (directoryTextBox is null)
        {
            return false;
        }

        var separatorPosition = directoryTextBox.Text.LastIndexOf(Path.DirectorySeparatorChar);
        if (separatorPosition < 0)
        {
            return false;
        }

        directoryTextBox.CaretIndex = directoryTextBox.Text.Length;
        var commonLength = directoryTextBox.Text.Length;
        while (!text.StartsWith(directoryTextBox.Text))
        {
            Keyboard.PressKey(window, Key.Back);
            commonLength--;
        }

        directoryTextBox.SendText(text[commonLength..]);

        return true;
    }
}