using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Views.Dialogs
{
    public class AboutDialogTest : IDisposable
    {
        private AboutDialog _dialog;

        [Fact(DisplayName = "Check if about dialog opens")]
        public void TestAboutDialog()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F1);

            _dialog = app
                .Windows
                .OfType<AboutDialog>()
                .SingleOrDefault();
            Assert.NotNull(_dialog);

            var githubButton = _dialog.GetVisualDescendants().OfType<Button>().SingleOrDefault();
            Assert.NotNull(githubButton);
            Assert.True(githubButton.Command.CanExecute(null));
        }

        public void Dispose() => _dialog?.Close();
    }
}