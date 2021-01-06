using Camelot.ViewModels.Implementations.Settings;
using Camelot.ViewModels.Interfaces.Settings;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Settings
{
    public class GeneralSettingsViewModelTests
    {
        [Theory]
        [InlineData(true, true, true)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, false, false)]
        public void TestProperties(bool isLanguageChanged, bool isThemeChanged, bool isChanged)
        {
            var languageSettingsViewModelMock = new Mock<ISettingsViewModel>();
            languageSettingsViewModelMock
                .SetupGet(m => m.IsChanged)
                .Returns(isLanguageChanged);
            var themeSettingsViewModelMock = new Mock<ISettingsViewModel>();
            themeSettingsViewModelMock
                .SetupGet(m => m.IsChanged)
                .Returns(isThemeChanged);
            var viewModel = new GeneralSettingsViewModel(languageSettingsViewModelMock.Object,
                themeSettingsViewModelMock.Object);

            Assert.Equal(languageSettingsViewModelMock.Object, viewModel.LanguageSettingsViewModel);
            Assert.Equal(themeSettingsViewModelMock.Object, viewModel.ThemeViewModel);
            Assert.Equal(isChanged, viewModel.IsChanged);
        }

        [Fact]
        public void TestMethods()
        {
            var languageSettingsViewModelMock = new Mock<ISettingsViewModel>();
            var themeSettingsViewModelMock = new Mock<ISettingsViewModel>();
            var viewModel = new GeneralSettingsViewModel(themeSettingsViewModelMock.Object,
                languageSettingsViewModelMock.Object);

            viewModel.Activate();
            languageSettingsViewModelMock
                .Verify(m => m.Activate(), Times.Once);
            themeSettingsViewModelMock
                .Verify(m => m.Activate(), Times.Once);

            viewModel.SaveChanges();
            languageSettingsViewModelMock
                .Verify(m => m.SaveChanges(), Times.Once);
            themeSettingsViewModelMock
                .Verify(m => m.SaveChanges(), Times.Once);
        }
    }
}