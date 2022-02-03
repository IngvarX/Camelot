using Avalonia.Input;
using Avalonia.Input.Raw;

namespace Camelot.Ui.Tests.Common;

public static class Keyboard
{
    public static void PressKey(IInputRoot inputRoot, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        KeyDown(inputRoot, key, modifiers);
        KeyUp(inputRoot, key, modifiers);
    }

    public static void KeyDown(IInputRoot inputRoot, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        KeyboardDevice.Instance.ProcessRawEvent(
            new RawKeyEventArgs(
                KeyboardDevice.Instance, 0, inputRoot,
                RawKeyEventType.KeyDown, key, modifiers));
    }

    public static void KeyUp(IInputRoot inputRoot, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        KeyboardDevice.Instance.ProcessRawEvent(
            new RawKeyEventArgs(
                KeyboardDevice.Instance, 0, inputRoot,
                RawKeyEventType.KeyUp, key, modifiers));
    }
}