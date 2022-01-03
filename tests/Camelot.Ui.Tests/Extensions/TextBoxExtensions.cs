using System;
using Avalonia.Controls;
using Avalonia.Input;

namespace Camelot.Ui.Tests.Extensions;

public static class TextBoxExtensions
{
    public static void SendText(this TextBox textBox, string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentNullException(nameof(text));
        }

        textBox.RaiseEvent(new TextInputEventArgs
        {
            Device = KeyboardDevice.Instance,
            Text = text,
            RoutedEvent = InputElement.TextInputEvent
        });
    }
}