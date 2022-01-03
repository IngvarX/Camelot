using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class ToggleFavouriteDirectoryStep
{
    public static void ToggleFavouriteDirectory(MainWindow window) =>
        Keyboard.PressKey(window, Key.D, RawInputModifiers.Control);
}