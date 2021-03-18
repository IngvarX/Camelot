using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Tests.Dialogs
{
    public class SettingsDialogTest : IUiTest, IDisposable
    {
        private SettingsDialog _dialog;

        public void Execute(Window mainWindow)
        {
            var menu = mainWindow.GetVisualDescendants().OfType<Menu>().First();
            var menuItem = menu.GetVisualDescendants().OfType<MenuItem>().Skip(1).First();
            menuItem.IsSubMenuOpen = true;
            var aboutMenuItem = menuItem.GetLogicalDescendants().OfType<MenuItem>().First();
            aboutMenuItem.Command?.Execute(null);

            _dialog = AvaloniaApp.GetApp().Windows.OfType<SettingsDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}