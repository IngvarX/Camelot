using System;
using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class GoToTabStep
{
    public static void GoToTab(MainWindow window, int tabIndex)
    {
        if (tabIndex <= 0 || tabIndex > 9)
        {
            throw new ArgumentOutOfRangeException(nameof(tabIndex));
        }

        Keyboard.PressKey(window, GetKey(tabIndex), RawInputModifiers.Control);
    }

    public static void GoToLastTab(MainWindow window) => Keyboard.PressKey(window, Key.D0, RawInputModifiers.Control);

    private static Key GetKey(int offset) => Key.D1 + offset;
}