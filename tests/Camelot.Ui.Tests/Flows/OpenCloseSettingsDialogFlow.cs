using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Flows
{
    public class OpenCloseSettingsDialogFlow : IDisposable
    {
        private SettingsDialog _dialog;

        [Fact(DisplayName = "Open and close settings dialog")]
        public async Task TestSettingsDialog()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            await Task.Delay(100);

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F2);

            _dialog = app
                .Windows
                .OfType<SettingsDialog>()
                .SingleOrDefault();
            Assert.NotNull(_dialog);

            await Task.Delay(100);

            var closeButton = _dialog
                .GetVisualDescendants()
                .OfType<Button>()
                .SingleOrDefault(b => b.Classes.Contains("transparentDialogButton"));
            Assert.NotNull(closeButton);

            Assert.True(closeButton.Command.CanExecute(null));
            closeButton.Command.Execute(null);

            await Task.Delay(100);

            _dialog = app
                .Windows
                .OfType<SettingsDialog>()
                .SingleOrDefault();
            Assert.Null(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}