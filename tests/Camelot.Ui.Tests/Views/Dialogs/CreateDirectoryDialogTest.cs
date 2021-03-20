using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Views.Dialogs
{
    public class CreateDirectoryDialogTest
    {
        [Fact(DisplayName = "Check if create directory dialog opens")]
        public void TestCreateDirectoryDialog()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F7);

            var dialog = app
                .Windows
                .OfType<CreateDirectoryDialog>()
                .SingleOrDefault();
            Assert.NotNull(dialog);

            var buttons = dialog
                .GetVisualDescendants()
                .OfType<Button>()
                .ToArray();
            Assert.Equal(2, buttons.Length);
            var createButton = buttons.SingleOrDefault(b => !b.Classes.Contains("transparentDialogButton"));

            Assert.NotNull(createButton);
            Assert.False(createButton.Command.CanExecute(null));
            Assert.True(createButton.IsDefault);

            var directoryNameTextBox = dialog
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

            dialog = app
                .Windows
                .OfType<CreateDirectoryDialog>()
                .SingleOrDefault();
            Assert.Null(dialog);
        }
    }
}