using System;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Tests.Dialogs
{
    public class AboutDialogTest : IUiTest, IDisposable
    {
        private AboutDialog _dialog;

        public void Execute(IClassicDesktopStyleApplicationLifetime app)
        {
            var window = app.MainWindow;

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F1);

            _dialog = app.Windows.OfType<AboutDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}