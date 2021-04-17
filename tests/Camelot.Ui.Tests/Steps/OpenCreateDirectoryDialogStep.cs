using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps
{
    public static class OpenCreateDirectoryDialogStep
    {
        public static void OpenCreateDirectoryDialog(MainWindow window)
        {
            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F7);
        }
    }
}