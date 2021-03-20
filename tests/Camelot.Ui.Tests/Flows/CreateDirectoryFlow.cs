using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Flows
{
    public class CreateDirectoryFlow : IDisposable
    {
        private const string DirectoryName = "CreateDirectoryTest__Directory";

        private CreateDirectoryDialog _dialog;

        [Fact(DisplayName = "Create and remove directory")]
        public void CreateAndRemoveDirectoryTest()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.F7);

            _dialog = app
                .Windows
                .OfType<CreateDirectoryDialog>()
                .Single();
            var directoryNameTextBox = _dialog
                .GetVisualDescendants()
                .OfType<TextBox>()
                .Single();

            directoryNameTextBox.RaiseEvent(new TextInputEventArgs
            {
                Device = KeyboardDevice.Instance,
                Text = DirectoryName,
                RoutedEvent = InputElement.TextInputEvent
            });
            Keyboard.PressKey(window, Key.Enter);

            _dialog = app
                .Windows
                .OfType<CreateDirectoryDialog>()
                .SingleOrDefault();
            Assert.Null(_dialog);
            // var filesPanel = app
            //     .MainWindow
            //     .GetVisualDescendants()
            //     .OfType<FilesPanelView>()
            //     .SingleOrDefault(fp => fp.IsFocused);
            // Assert.NotNull(filesPanel);
        }

        public void Dispose() => _dialog?.Close();
    }
}