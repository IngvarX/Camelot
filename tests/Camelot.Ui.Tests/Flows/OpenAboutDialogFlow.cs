using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Flows
{
    public class OpenAboutDialogFlow : IDisposable
    {
        private AboutDialog _dialog;

        [Fact(DisplayName = "Open about dialog")]
        public async Task TestAboutDialog()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            await Task.Delay(100);

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F1);

            await Task.Delay(100);

            _dialog = app
                .Windows
                .OfType<AboutDialog>()
                .SingleOrDefault();
            Assert.NotNull(_dialog);

            await Task.Delay(100);

            var githubButton = _dialog.GetVisualDescendants().OfType<Button>().SingleOrDefault();
            Assert.NotNull(githubButton);
            Assert.True(githubButton.IsDefault);
            Assert.True(githubButton.Command.CanExecute(null));
        }

        public void Dispose() => _dialog?.Close();
    }
}