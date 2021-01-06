using System.Collections.Generic;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Implementations;
using Camelot.ViewModels.Services.Interfaces;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Factories
{
    public class ThemeViewModelFactoryTests
    {
        private const string DarkResourceName = "DarkResourceName";
        private const string DarkThemeName = "DarkThemeName";
        private const string LightThemeName = "LightThemeName";
        private const string LightResourceName = "LightResourceName";

        private readonly AutoMocker _autoMocker;

        public ThemeViewModelFactoryTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestCreateAll()
        {
            var config = new ThemesNamesConfiguration
            {
                ThemeToResourceMapping = new Dictionary<Theme, string>
                {
                    [Theme.Dark] = DarkResourceName,
                    [Theme.Light] = LightResourceName
                }
            };
            _autoMocker.Use(config);
            _autoMocker
                .Setup<IResourceProvider, string>(m => m.GetResourceByName(DarkResourceName))
                .Returns(DarkThemeName);
            _autoMocker
                .Setup<IResourceProvider, string>(m => m.GetResourceByName(LightResourceName))
                .Returns(LightThemeName);

            var factory = _autoMocker.CreateInstance<ThemeViewModelFactory>();
            var viewModels = factory.CreateAll();

            Assert.NotNull(viewModels);
            Assert.Equal(2, viewModels.Count);

            var darkThemeVm = viewModels[0];
            var lightThemeVm = viewModels[1];

            Assert.NotNull(darkThemeVm);
            Assert.NotNull(lightThemeVm);

            Assert.Equal(Theme.Dark, darkThemeVm.Theme);
            Assert.Equal(Theme.Light, lightThemeVm.Theme);

            Assert.Equal(DarkThemeName, darkThemeVm.ThemeName);
            Assert.Equal(LightThemeName, lightThemeVm.ThemeName);
        }
    }
}