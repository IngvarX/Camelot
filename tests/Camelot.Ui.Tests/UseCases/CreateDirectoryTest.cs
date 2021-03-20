// using System.Linq;
// using Avalonia.Controls;
// using Avalonia.Input;
// using Avalonia.VisualTree;
// using Camelot.Views.Dialogs;
// using Camelot.Views.Main;
// using Xunit;
//
// namespace Camelot.Ui.Tests.UseCases
// {
//     public class CreateDirectoryTest
//     {
//         private const string DirectoryName = "CreateDirectoryTest__Directory";
//
//         [Fact(DisplayName = "Create and remove directory")]
//         public void CreateAndRemoveDirectoryTest()
//         {
//             var app = AvaloniaApp.GetApp();
//             var window = AvaloniaApp.GetMainWindow();
//
//             Keyboard.PressKey(window, Key.Tab);
//             Keyboard.PressKey(window, Key.Down);
//             Keyboard.PressKey(window, Key.F7);
//
//             var dialog = app
//                 .Windows
//                 .OfType<CreateDirectoryDialog>()
//                 .Single();
//             var directoryNameTextBox = dialog
//                 .GetVisualDescendants()
//                 .OfType<TextBox>()
//                 .Single();
//
//             directoryNameTextBox.RaiseEvent(new TextInputEventArgs
//             {
//                 Device = KeyboardDevice.Instance,
//                 Text = DirectoryName,
//                 RoutedEvent = InputElement.TextInputEvent
//             });
//             Keyboard.PressKey(window, Key.Enter);
//
//             var filesPanel = app
//                 .MainWindow
//                 .GetVisualDescendants()
//                 .OfType<FilesPanelView>()
//                 .SingleOrDefault(fp => fp.IsFocused);
//             Assert.NotNull(filesPanel);
//         }
//     }
// }