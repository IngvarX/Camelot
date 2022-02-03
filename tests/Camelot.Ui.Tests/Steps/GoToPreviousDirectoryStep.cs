using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class GoToPreviousDirectoryStep
{
    public static void GoToPreviousDirectory(MainWindow window) =>
        Keyboard.PressKey(window, Key.Left);
}