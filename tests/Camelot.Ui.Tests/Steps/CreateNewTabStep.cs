using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class CreateNewTabStep
{
    public static void CreateNewTab(MainWindow window) =>
        Keyboard.PressKey(window, Key.T, RawInputModifiers.Control);
}