using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Flows
{
    public class OpenCloseCreateDirectoryDialogFlow : IDisposable
    {
        private CreateDirectoryDialog _dialog;

        [Fact(DisplayName = "Open and close create directory dialog")]
        public void TestCreateDirectoryDialog()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F7);

            _dialog = app
                .Windows
                .OfType<CreateDirectoryDialog>()
                .SingleOrDefault();
            Assert.NotNull(_dialog);

            var buttons = _dialog
                .GetVisualDescendants()
                .OfType<Button>()
                .ToArray();
            Assert.Equal(2, buttons.Length);
            var createButton = buttons.SingleOrDefault(b => !b.Classes.Contains("transparentDialogButton"));

            Assert.NotNull(createButton);
            Assert.False(createButton.Command.CanExecute(null));
            Assert.True(createButton.IsDefault);

            var directoryNameTextBox = _dialog
                .GetVisualDescendants()
                .OfType<TextBox>()
                .SingleOrDefault();
            Assert.NotNull(directoryNameTextBox);
            Assert.True(string.IsNullOrEmpty(directoryNameTextBox.Text));
            Assert.True(directoryNameTextBox.IsFocused);

            directoryNameTextBox.RaiseEvent(new TextInputEventArgs
            {
                Device = KeyboardDevice.Instance,
                Text = "DirectoryName",
                RoutedEvent = InputElement.TextInputEvent
            });

            Assert.True(createButton.Command.CanExecute(null));

            var closeButton = buttons.SingleOrDefault(b => b.Classes.Contains("transparentDialogButton"));
            Assert.NotNull(closeButton);
            Assert.True(closeButton.Command.CanExecute(null));
            Assert.False(closeButton.IsDefault);

            closeButton.Command.Execute(null);

            _dialog = app
                .Windows
                .OfType<CreateDirectoryDialog>()
                .SingleOrDefault();
            Assert.Null(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}