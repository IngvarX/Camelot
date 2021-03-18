using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests
{
    public class MainWindowTests : IDisposable
    {
        private const int DelaySeconds = 3;

        public MainWindowTests()
        {
            AvaloniaApp.RegisterDependencies();
        }

        [Fact]
        public void TestOpenAbout()
        {
            var dialogExists = false;

            AvaloniaApp
                .BuildAvaloniaApp()
                .AfterSetup(_ =>
                {
                    DispatcherTimer.RunOnce(async () =>
                    {
                        try
                        {
                            var window = AvaloniaApp.GetMainWindow();
                            var menu = window.GetVisualDescendants().OfType<Menu>().First();
                            var menuItem = menu.GetVisualDescendants().OfType<MenuItem>().Skip(2).First();
                            menuItem.IsSubMenuOpen = true;
                            var aboutMenuItem = menuItem.GetLogicalDescendants().OfType<MenuItem>().First();
                            aboutMenuItem.Command?.Execute(null);
                            // var window = AvaloniaApp.GetMainWindow();
                            // var menu = window.GetVisualDescendants().OfType<Menu>().First();
                            // menu.RaiseEvent(new KeyEventArgs
                            // {
                            //     Key = Key.H,
                            //     KeyModifiers = KeyModifiers.Alt,
                            //     RoutedEvent = new RoutedEvent("name", RoutingStrategies.Bubble,
                            //          typeof(KeyEventArgs), window.GetType())
                            // });
                            // await Task.Delay(10000);
                            // menu.RaiseEvent(new KeyEventArgs
                            // {
                            //     Key = Key.A,
                            //     RoutedEvent = new RoutedEvent("name", RoutingStrategies.Bubble,
                            //         typeof(RoutedEventArgs), window.GetType())
                            // });
                            //
                            //

                            await Task.Delay(10000);

                            var dialog = AvaloniaApp.GetApp().Windows.OfType<AboutDialog>().SingleOrDefault();
                            dialogExists = dialog != null;
                        }
                        finally
                        {
                            AvaloniaApp.Stop();
                        }
                    }, TimeSpan.FromSeconds(DelaySeconds));
                })
                .StartWithClassicDesktopLifetime(new string[0]);

            Assert.True(dialogExists);
        }

        [Fact]
        public void TestOpenSettings()
        {
            var dialogExists = false;

            AvaloniaApp
                .BuildAvaloniaApp()
                .AfterSetup(_ =>
                {
                    DispatcherTimer.RunOnce(async () =>
                    {
                        try
                        {
                            var window = AvaloniaApp.GetMainWindow();
                            var menu = window.GetVisualDescendants().OfType<Menu>().First();
                            var menuItem = menu.GetVisualDescendants().OfType<MenuItem>().Skip(1).First();
                            menuItem.IsSubMenuOpen = true;
                            var aboutMenuItem = menuItem.GetLogicalDescendants().OfType<MenuItem>().First();
                            aboutMenuItem.Command?.Execute(null);
                            var dialog = AvaloniaApp.GetApp().Windows.OfType<SettingsDialog>().SingleOrDefault();
                            dialogExists = dialog != null;
                        }
                        finally
                        {
                            AvaloniaApp.Stop();
                        }
                    }, TimeSpan.FromSeconds(DelaySeconds));

                })
                .StartWithClassicDesktopLifetime(new string[0]);

            Assert.True(dialogExists);
        }

        public void Dispose() => AvaloniaApp.Stop();
    }
}