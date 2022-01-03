using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class OpenRemoveDialogStep
{
    public static void OpenRemoveDialog(MainWindow window) => Keyboard.PressKey(window, Key.F8);

    public static void OpenPermanentRemoveDialog(MainWindow window) =>
        Keyboard.PressKey(window, Key.Back, RawInputModifiers.Control);
}