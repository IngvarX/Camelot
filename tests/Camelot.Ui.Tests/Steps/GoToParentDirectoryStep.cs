using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class GoToParentDirectoryStep
{
    public static void GoToParentDirectoryViaFilePanel(MainWindow window)
    {
        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        Keyboard.PressKey(window, Key.Down);
        Keyboard.PressKey(window, Key.Up);
        Keyboard.PressKey(window, Key.Up);
        Keyboard.PressKey(window, Key.Enter);
    }
}