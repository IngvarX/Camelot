using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Extensions;
using Camelot.Views;
using Camelot.Views.Dialogs;

namespace Camelot.Ui.Tests.Steps;

public static class CreateDirectoryStep
{
    public static void CreateDirectory(IClassicDesktopStyleApplicationLifetime app,
        MainWindow window, string directoryName)
    {
        var createDirectoryDialog = app
            .Windows
            .OfType<CreateDirectoryDialog>()
            .Single();
        var directoryNameTextBox = createDirectoryDialog
            .GetVisualDescendants()
            .OfType<TextBox>()
            .Single();

        directoryNameTextBox.SendText(directoryName);
        Keyboard.PressKey(window, Key.Enter);
    }
}