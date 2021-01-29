using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Configuration;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.Tests
{
    public class ThemeServiceTests
    {
        private const string ThemeSettingsId = "ThemeSettings";

        private readonly AutoMocker _autoMocker;

        public ThemeServiceTests()
        {
            _autoMocker = new AutoMocker();
        }

        [Theory]
        [InlineData(0, Theme.Dark)]
        [InlineData(1, Theme.Light)]
        public void TestGet(int themeId, Theme theme)
        {
            var repositoryMock = new Mock<IRepository<ThemeSettings>>();
            repositoryMock
                .Setup(m => m.GetById(ThemeSettingsId))
                .Returns(new ThemeSettings {SelectedTheme = themeId})
                .Verifiable();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.GetRepository<ThemeSettings>())
                .Returns(repositoryMock.Object);

            _autoMocker
                .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
                .Returns(unitOfWorkMock.Object);

            var themeService = _autoMocker.CreateInstance<ThemeService>();
            var themeSettings = themeService.GetThemeSettings();

            Assert.Equal(theme, themeSettings.SelectedTheme);

            repositoryMock
                .Verify(m => m.GetById(ThemeSettingsId), Times.Once);
        }

        [Theory]
        [InlineData(0, Theme.Light, Theme.Dark)]
        [InlineData(0, Theme.Dark, Theme.Dark)]
        [InlineData(1, Theme.Dark, Theme.Light)]
        [InlineData(1, Theme.Light, Theme.Light)]
        [InlineData(null, Theme.Dark, Theme.Dark)]
        [InlineData(null, Theme.Light, Theme.Light)]
        public void TestGetCurrentTheme(int? themeId, Theme configTheme, Theme theme)
        {
            var repositoryMock = new Mock<IRepository<ThemeSettings>>();
            repositoryMock
                .Setup(m => m.GetById(ThemeSettingsId))
                .Returns(themeId.HasValue ? new ThemeSettings {SelectedTheme = themeId.Value} : null);

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.GetRepository<ThemeSettings>())
                .Returns(repositoryMock.Object);

            _autoMocker
                .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
                .Returns(unitOfWorkMock.Object);
            var defaultConfig = new DefaultThemeConfiguration
            {
                DefaultTheme = configTheme
            };
            _autoMocker.Use(defaultConfig);

            var themeService = _autoMocker.CreateInstance<ThemeService>();
            var currentTheme = themeService.GetCurrentTheme();

            Assert.Equal(theme, currentTheme);
        }

        [Theory]
        [InlineData(0, Theme.Dark)]
        [InlineData(1, Theme.Light)]
        public void TestSave(int themeId, Theme theme)
        {
            var repositoryMock = new Mock<IRepository<ThemeSettings>>();
            repositoryMock
                .Setup(m => m.Upsert(ThemeSettingsId,
                    It.Is<ThemeSettings>(l => l.SelectedTheme == themeId)))
                .Verifiable();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.GetRepository<ThemeSettings>())
                .Returns(repositoryMock.Object);
            _autoMocker
                .Setup<IUnitOfWorkFactory, IUnitOfWork>(m => m.Create())
                .Returns(unitOfWorkMock.Object);

            var themeService = _autoMocker.CreateInstance<ThemeService>();
            var themeSettingsModel = new ThemeSettingsModel(theme);

            themeService.SaveThemeSettings(themeSettingsModel);

            repositoryMock
                .Verify(m => m.Upsert(ThemeSettingsId,
                        It.Is<ThemeSettings>(l => l.SelectedTheme == themeId)),
                    Times.Once);
        }

        [Fact]
        public void TestSaveFails()
        {
            var themeService = _autoMocker.CreateInstance<ThemeService>();

            Assert.Throws<ArgumentNullException>(() => themeService.SaveThemeSettings(null));
        }
    }
}