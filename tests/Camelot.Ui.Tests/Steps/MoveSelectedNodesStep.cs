using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Views;

namespace Camelot.Ui.Tests.Steps;

public static class MoveSelectedNodesStep
{
    public static void MoveSelectedNodes(MainWindow window) =>
        Keyboard.PressKey(window, Key.F6);
}