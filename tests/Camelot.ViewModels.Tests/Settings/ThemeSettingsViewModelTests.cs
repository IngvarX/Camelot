using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Settings.General;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Settings
{
    public class ThemeSettingsViewModelTests
    {
        private readonly AutoMocker _autoMocker;

        public ThemeSettingsViewModelTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Fact]
        public void TestActivation()
        {
            var themes = new[]
            {
                new ThemeViewModel(Theme.Dark, "Dark"),
                new ThemeViewModel(Theme.Light, "Light")
            };
            _autoMocker
                .Setup<IThemeViewModelFactory, IReadOnlyList<ThemeViewModel>>(m => m.CreateAll())
                .Returns(themes)
                .Verifiable();
            _autoMocker
                .Setup<IThemeService, Theme>(m => m.GetCurrentTheme())
                .Returns(Theme.Light);

            var viewModel = _autoMocker.CreateInstance<ThemeSettingsViewModel>();

            for (var i = 0; i < 10; i++)
            {
                viewModel.Activate();
            }

            Assert.Equal(themes.Last(), viewModel.CurrentTheme);
            Assert.False(viewModel.IsChanged);
            Assert.Equal(2, viewModel.Themes.Count());
            Assert.Equal(themes.First(), viewModel.Themes.First());
            Assert.Equal(themes.Last(), viewModel.Themes.Last());

            _autoMocker
                .Verify<IThemeViewModelFactory, IReadOnlyList<ThemeViewModel>>(m => m.CreateAll(),
                    Times.Once);
        }

        [Fact]
        public void TestSave()
        {
            var themes = new[]
            {
                new ThemeViewModel(Theme.Dark, "Dark"),
                new ThemeViewModel(Theme.Light, "Light")
            };
            _autoMocker
                .Setup<IThemeViewModelFactory, IReadOnlyList<ThemeViewModel>>(m => m.CreateAll())
                .Returns(themes)
                .Verifiable();
            _autoMocker
                .Setup<IThemeService, Theme>(m => m.GetCurrentTheme())
                .Returns(Theme.Light);
            _autoMocker
                .Setup<IThemeService>(m => m.SaveThemeSettings(It.Is<ThemeSettingsModel>(s =>
                    s.SelectedTheme == Theme.Dark)))
                .Verifiable();

            var viewModel = _autoMocker.CreateInstance<ThemeSettingsViewModel>();

            viewModel.Activate();
            viewModel.CurrentTheme = viewModel.Themes.First();
            Assert.True(viewModel.IsChanged);
            viewModel.SaveChanges();

            _autoMocker
                .Verify<IThemeService>(m => m.SaveThemeSettings(It.Is<ThemeSettingsModel>(s =>
                    s.SelectedTheme == Theme.Dark)), Times.Once());
        }
    }
}