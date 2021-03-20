using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Views.Main.Controls.Tabs;
using Xunit;

namespace Camelot.Ui.Tests.Flows
{
    public class CreateAndCloseNewTabFlow
    {
        [Fact(DisplayName = "Create and close tab")]
        public async Task TestAboutDialog()
        {
            var window = AvaloniaApp.GetMainWindow();

            await Task.Delay(100);

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);

            var initialCount = GetTabsCount(window);

            for (var i = 0; i < 2; i++)
            {
                Keyboard.PressKey(window, Key.T, RawInputModifiers.Control);
                await Task.Delay(300);
                Assert.Equal(initialCount + 1,  GetTabsCount(window));

                Keyboard.PressKey(window, Key.W, RawInputModifiers.Control);
                await Task.Delay(300);
                Assert.Equal(initialCount, GetTabsCount(window));

                Keyboard.PressKey(window, Key.Tab);
            }
        }

        private static int GetTabsCount(IVisual window) =>
            window
                .GetVisualDescendants()
                .OfType<TabView>()
                .Count();
    }
}