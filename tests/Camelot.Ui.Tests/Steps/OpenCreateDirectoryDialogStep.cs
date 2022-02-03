using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class OpenCreateDirectoryDialogStep
{
    public static void OpenCreateDirectoryDialog(MainWindow window) => Keyboard.PressKey(window, Key.F7);
}