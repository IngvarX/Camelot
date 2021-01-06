using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Implementations.Settings.General;
using Xunit;

namespace Camelot.ViewModels.Tests.Settings
{
    public class ThemeViewModelTests
    {
        private const Theme DefaultTheme = Theme.Light;
        private const string DefaultName = "Light";

        [Fact]
        public void TestProperties()
        {
            var vm = new ThemeViewModel(DefaultTheme, DefaultName);

            Assert.Equal(DefaultName, vm.ThemeName);
            Assert.Equal(DefaultTheme, vm.Theme);
        }
    }
}