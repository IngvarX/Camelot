using System;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Tests.Dialogs
{
    public class CreateDirectoryTest : IUiTest, IDisposable
    {
        private CreateDirectoryDialog _dialog;

        public void Execute(IClassicDesktopStyleApplicationLifetime app)
        {
            var window = app.MainWindow;
            KeyboardDevice.Instance.ProcessRawEvent(
                new RawKeyEventArgs(
                    KeyboardDevice.Instance, 0,  window,
                    RawKeyEventType.KeyDown, Key.Tab, RawInputModifiers.None));
            KeyboardDevice.Instance.ProcessRawEvent(
                new RawKeyEventArgs(
                    KeyboardDevice.Instance, 0,  window,
                    RawKeyEventType.KeyUp, Key.Tab, RawInputModifiers.None));
            KeyboardDevice.Instance.ProcessRawEvent(
                new RawKeyEventArgs(
                    KeyboardDevice.Instance, 0,  window,
                    RawKeyEventType.KeyDown, Key.Down, RawInputModifiers.None));
            KeyboardDevice.Instance.ProcessRawEvent(
                new RawKeyEventArgs(
                    KeyboardDevice.Instance, 0,  window,
                    RawKeyEventType.KeyDown, Key.Up, RawInputModifiers.None));
            KeyboardDevice.Instance.ProcessRawEvent(
                new RawKeyEventArgs(
                    KeyboardDevice.Instance, 0,  window,
                    RawKeyEventType.KeyDown, Key.F7, RawInputModifiers.None));

            _dialog = app.Windows.OfType<CreateDirectoryDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}