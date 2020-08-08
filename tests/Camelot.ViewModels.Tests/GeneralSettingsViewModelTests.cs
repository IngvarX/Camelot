using Camelot.ViewModels.Implementations.Settings;
using Camelot.ViewModels.Interfaces.Settings;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests
{
    public class GeneralSettingsViewModelTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestProperties(bool isChanged)
        {
            var settingsViewModelMock = new Mock<ISettingsViewModel>();
            settingsViewModelMock
                .SetupGet(m => m.IsChanged)
                .Returns(isChanged);
            var viewModel = new GeneralSettingsViewModel(settingsViewModelMock.Object);

            Assert.Equal(settingsViewModelMock.Object, viewModel.LanguageSettingsViewModel);
            Assert.Equal(isChanged, viewModel.IsChanged);
        }

        [Fact]
        public void TestMethods()
        {
            var settingsViewModelMock = new Mock<ISettingsViewModel>();
            settingsViewModelMock
                .Setup(m => m.Activate())
                .Verifiable();
            settingsViewModelMock
                .Setup(m => m.SaveChanges())
                .Verifiable();
            var viewModel = new GeneralSettingsViewModel(settingsViewModelMock.Object);

            viewModel.Activate();
            settingsViewModelMock
                .Verify(m => m.Activate(), Times.Once);

            viewModel.SaveChanges();
            settingsViewModelMock
                .Verify(m => m.SaveChanges(), Times.Once);
        }
    }
}