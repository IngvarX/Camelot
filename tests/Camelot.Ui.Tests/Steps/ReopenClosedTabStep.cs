using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class ReopenClosedTabStep
{
    public static void ReopenClosedTab(MainWindow window) =>
        Keyboard.PressKey(window, Key.T, RawInputModifiers.Control | RawInputModifiers.Shift);
}