using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class GoToNextDirectoryStep
{
    public static void GoToNextDirectory(MainWindow window) =>
        Keyboard.PressKey(window, Key.Right);
}