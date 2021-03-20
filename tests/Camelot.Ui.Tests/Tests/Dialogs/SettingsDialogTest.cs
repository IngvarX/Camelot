using System;
using System.Linq;
using Avalonia.Input;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Tests.Dialogs
{
    public class SettingsDialogTest : IDisposable
    {
        private SettingsDialog _dialog;

        [Fact]
        public void Execute()
        {
            var app = AvaloniaApp.GetApp();
            var window = app.MainWindow;

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F2);

            _dialog = app.Windows.OfType<SettingsDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}