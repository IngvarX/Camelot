using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class FocusDirectorySelectorStep
{
    public static void FocusDirectorySelector(MainWindow window) =>
        Keyboard.PressKey(window, Key.L, RawInputModifiers.Control);
}