using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Tests.Dialogs
{
    public class CreateDirectoryTest : IDisposable
    {
        private CreateDirectoryDialog _dialog;

        [Fact]
        public void Execute()
        {
            var app = AvaloniaApp.GetApp();
            var window = app.MainWindow;

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F7);

            _dialog = app.Windows.OfType<CreateDirectoryDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);

            var createButton = _dialog
                .GetVisualDescendants()
                .OfType<Button>()
                .SingleOrDefault(b => !b.Classes.Contains("transparentDialogButton"));

            Assert.NotNull(createButton);
            Assert.False(createButton.Command.CanExecute(null));

            var directoryNameTextBox = _dialog
                .GetVisualDescendants()
                .OfType<TextBox>()
                .SingleOrDefault();
            Assert.NotNull(directoryNameTextBox);
            Assert.True(string.IsNullOrEmpty(directoryNameTextBox.Text));

            directoryNameTextBox.RaiseEvent(new TextInputEventArgs
            {
                Device = KeyboardDevice.Instance,
                Text = "DirectoryName",
                RoutedEvent = InputElement.TextInputEvent
            });

            Assert.True(createButton.Command.CanExecute(null));
            Keyboard.PressKey(_dialog, Key.Enter);
        }

        public void Dispose() => _dialog?.Close();
    }
}