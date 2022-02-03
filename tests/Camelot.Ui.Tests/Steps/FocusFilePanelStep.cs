using System.Threading.Tasks;
using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class FocusFilePanelStep
{
    public static async Task FocusFilePanelAsync(MainWindow window)
    {
        await Task.Delay(100);

        Keyboard.PressKey(window, Key.Tab);
        Keyboard.PressKey(window, Key.Down);
    }
}