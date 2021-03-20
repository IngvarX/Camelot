using System;
using System.Linq;
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

            var closeButton = _dialog
                .GetVisualDescendants()
                .OfType<Button>()
                .SingleOrDefault(b => b.Classes.Contains("transparentDialogButton"));
            Assert.NotNull(closeButton);

            Assert.True(closeButton.Command.CanExecute(null));
            closeButton.Command.Execute(null);

            _dialog = app
                .Windows
                .OfType<SettingsDialog>()
                .SingleOrDefault();
            Assert.Null(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}