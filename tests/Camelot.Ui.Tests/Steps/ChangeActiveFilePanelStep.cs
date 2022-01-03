using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class ChangeActiveFilePanelStep
{
    public static void ChangeActiveFilePanel(MainWindow window) =>
        Keyboard.PressKey(window, Key.Tab);
}