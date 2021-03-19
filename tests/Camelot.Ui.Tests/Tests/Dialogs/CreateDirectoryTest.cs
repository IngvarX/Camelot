using System;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
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

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F7);

            _dialog = app.Windows.OfType<CreateDirectoryDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}