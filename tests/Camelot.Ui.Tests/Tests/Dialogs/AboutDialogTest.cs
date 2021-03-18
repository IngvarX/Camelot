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
    public class AboutDialogTest : IUiTest, IDisposable
    {
        private AboutDialog _dialog;

        public void Execute(IClassicDesktopStyleApplicationLifetime app)
        {
            var menu = app.MainWindow.GetVisualDescendants().OfType<Menu>().First();
            var menuItem = menu.GetVisualDescendants().OfType<MenuItem>().Skip(2).First();
            menuItem.IsSubMenuOpen = true;
            var aboutMenuItem = menuItem.GetLogicalDescendants().OfType<MenuItem>().First();
            aboutMenuItem.Command?.Execute(null);

            _dialog = app.Windows.OfType<AboutDialog>().SingleOrDefault();
            Assert.NotNull(_dialog);

            var githubButton = _dialog.GetVisualDescendants().OfType<Button>().SingleOrDefault();
            Assert.NotNull(githubButton);
        }

        public void Dispose() => _dialog?.Close();
    }
}