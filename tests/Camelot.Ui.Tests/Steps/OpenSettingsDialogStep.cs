using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class OpenSettingsDialogStep
{
    public static void OpenSettingsDialog(MainWindow window) => Keyboard.PressKey(window, Key.F2);
}