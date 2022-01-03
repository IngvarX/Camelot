using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class CopySelectedNodesStep
{
    public static void CopySelectedNodes(MainWindow window) =>
        Keyboard.PressKey(window, Key.F5);
}