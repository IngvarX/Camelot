using Avalonia.Input;
using Avalonia.Input.Raw;

namespace Camelot.Ui.Tests
{
    public static class Keyboard
    {
        public static void PressKey(IInputRoot inputRoot, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
        {
            KeyboardDevice.Instance.ProcessRawEvent(
                new RawKeyEventArgs(
                    KeyboardDevice.Instance, 0, inputRoot,
                    RawKeyEventType.KeyDown, key, modifiers));

            KeyboardDevice.Instance.ProcessRawEvent(
                new RawKeyEventArgs(
                    KeyboardDevice.Instance, 0, inputRoot,
                    RawKeyEventType.KeyUp, key, modifiers));
        }
    }
}