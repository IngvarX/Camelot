using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class CloseCurrentTabStep
{
    public static void CloseCurrentTab(MainWindow window) =>
        Keyboard.PressKey(window, Key.W, RawInputModifiers.Control);
}