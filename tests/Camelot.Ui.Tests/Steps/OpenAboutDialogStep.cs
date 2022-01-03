using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class OpenAboutDialogStep
{
    public static void OpenAboutDialog(MainWindow window) => Keyboard.PressKey(window, Key.F1);
}