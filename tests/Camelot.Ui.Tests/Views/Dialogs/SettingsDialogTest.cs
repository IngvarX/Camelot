using System;
using System.Linq;
using Avalonia.Input;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Views.Dialogs
{
    public class SettingsDialogTest : IDisposable
    {
        private SettingsDialog _dialog;

        [Fact(DisplayName = "Check if create settings dialog opens")]
        public void TestSettingsDialog()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F2);

            _dialog = app
                .Windows
                .OfType<SettingsDialog>()
                .SingleOrDefault();
            Assert.NotNull(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}