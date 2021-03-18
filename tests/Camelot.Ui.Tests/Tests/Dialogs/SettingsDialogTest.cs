using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Tests.Dialogs
{
    public class SettingsDialogTest : IUiTest, IDisposable
    {
        private SettingsDialog _dialog;

        public void Execute(IClassicDesktopStyleApplicationLifetime app)
        {
            var menu = app.MainWindow.GetVisualDescendants().OfType<Menu>().First();
            var menuItem = menu.GetVisualDescendants().OfType<MenuItem>().Skip(1).First();
            menuItem.IsSubMenuOpen = true;
            var settingsMenuItem = menuItem.GetLogicalDescendants().OfType<MenuItem>().First();
            settingsMenuItem.Command?.Execute(null);

            _dialog = app.Windows.OfType<SettingsDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);
        }

        public void Dispose() => _dialog?.Close();
    }
}