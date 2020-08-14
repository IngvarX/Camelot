using System.Linq;
using Camelot.DataAccess.Models;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Implementations.Settings.General;
using Moq;
using Xunit;

namespace Camelot.ViewModels.Tests.Settings
{
    public class LanguageSettingsViewModelTests
    {
        [Fact]
        public void TestActivation()
        {
            var languages = new[]
            {
                new LanguageModel("test", "test", "t1"),
                new LanguageModel("test2", "test2", "t2")
            };
            var localizationServiceMock = new Mock<ILocalizationService>();
            localizationServiceMock
                .Setup(m => m.GetSavedLanguage())
                .Returns(new Language
                {
                    Name = languages.Last().Name,
                    Code = languages.Last().Code
                });
            var languageManagerMock = new Mock<ILanguageManager>();
            languageManagerMock
                .SetupGet(m => m.AllLanguages)
                .Returns(languages);
            var viewModel = new LanguageSettingsViewModel(localizationServiceMock.Object, languageManagerMock.Object);

            viewModel.Activate();
            viewModel.Activate();

            Assert.Equal(languages.Last(), viewModel.CurrentLanguage);
            Assert.False(viewModel.IsChanged);
            Assert.Equal(2, viewModel.Languages.Count());
            Assert.Equal(languages.First(), viewModel.Languages.First());
            Assert.Equal(languages.Last(), viewModel.Languages.Last());
        }

        [Fact]
        public void TestSave()
        {
            var languageModels = new[]
            {
                new LanguageModel("test", "test", "t1"),
                new LanguageModel("test2", "test2", "t2")
            };
            var language = new Language
            {
                Name = languageModels.Last().Name,
                Code = languageModels.Last().Code
            };
            var localizationServiceMock = new Mock<ILocalizationService>();
            localizationServiceMock
                .Setup(m => m.GetSavedLanguage())
                .Returns(language);
            localizationServiceMock
                .Setup(m => m.SaveLanguage(languageModels.First()))
                .Verifiable();
            var languageManagerMock = new Mock<ILanguageManager>();
            languageManagerMock
                .SetupGet(m => m.AllLanguages)
                .Returns(languageModels);
            languageManagerMock
                .Setup(m => m.SetLanguage(languageModels.First()))
                .Verifiable();
            var viewModel = new LanguageSettingsViewModel(localizationServiceMock.Object, languageManagerMock.Object);

            viewModel.Activate();
            viewModel.CurrentLanguage = viewModel.Languages.First();
            Assert.True(viewModel.IsChanged);
            viewModel.SaveChanges();

            languageManagerMock
                .Verify(m => m.SetLanguage(languageModels.First()), Times.Once);
            localizationServiceMock
                .Verify(m => m.SaveLanguage(languageModels.First()), Times.Once);
        }
    }
}