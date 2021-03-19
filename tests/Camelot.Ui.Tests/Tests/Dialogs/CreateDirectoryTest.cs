using System;
using System.Linq;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Camelot.Avalonia.Implementations;
using Camelot.Views.Dialogs;
using Camelot.Views.Main;
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
                    RawKeyEventType.KeyDown, Key.F7, RawInputModifiers.None));

            _dialog = app.Windows.OfType<CreateDirectoryDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}