using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class ToggleSearchPanelStep
{
    public static void ToggleSearchPanelVisibility(MainWindow window) =>
        Keyboard.PressKey(window, Key.F, RawInputModifiers.Control);
}