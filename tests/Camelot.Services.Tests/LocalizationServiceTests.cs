using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions.Models.State;
using Moq;
using Xunit;

namespace Camelot.Services.Tests
{
    public class LocalizationServiceTests
    {
        private const string LanguageSettingsId = "LanguageSettings";
        private const string LanguageName = "Name";
        private const string LanguageCode = "Code";

        [Fact]
        public void TestGetLanguage()
        {
            var repositoryMock = new Mock<IRepository<Language>>();
            repositoryMock
                .Setup(m => m.GetById(LanguageSettingsId))
                .Returns(new Language {Name = LanguageName, Code = LanguageCode})
                .Verifiable();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.GetRepository<Language>())
                .Returns(repositoryMock.Object);
            var unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            unitOfWorkFactoryMock
                .Setup(m => m.Create())
                .Returns(unitOfWorkMock.Object);

            var localizationService = new LocalizationService(unitOfWorkFactoryMock.Object);
            var language = localizationService.GetSavedLanguage();

            Assert.Equal(LanguageName, language.Name);
            Assert.Equal(LanguageCode, language.Code);

            repositoryMock.Verify(m => m.GetById(LanguageSettingsId), Times.Once);
        }

        [Fact]
        public void TestSaveLanguage()
        {
            var repositoryMock = new Mock<IRepository<Language>>();
            repositoryMock
                .Setup(m => m.Upsert(LanguageSettingsId,
                    It.Is<Language>(l => l.Code == LanguageCode && l.Name == LanguageName)))
                .Verifiable();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock
                .Setup(m => m.GetRepository<Language>())
                .Returns(repositoryMock.Object);
            var unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            unitOfWorkFactoryMock
                .Setup(m => m.Create())
                .Returns(unitOfWorkMock.Object);

            var localizationService = new LocalizationService(unitOfWorkFactoryMock.Object);
            var languageModel = CreateFrom(LanguageName, LanguageCode);
            localizationService.SaveLanguage(languageModel);

            repositoryMock
                .Verify(m => m.Upsert(LanguageSettingsId,
                    It.Is<Language>(l => l.Code == LanguageCode && l.Name == LanguageName)), Times.Once);
        }

        [Fact]
        public void TestSaveLanguageFails()
        {
            var unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
            var localizationService = new LocalizationService(unitOfWorkFactoryMock.Object);

            Assert.Throws<ArgumentNullException>(() => localizationService.SaveLanguage(null));
            Assert.Throws<ArgumentException>(() => localizationService.SaveLanguage(CreateFrom("", "")));
            Assert.Throws<ArgumentException>(() => localizationService.SaveLanguage(CreateFrom(null, "")));
            Assert.Throws<ArgumentException>(() => localizationService.SaveLanguage(CreateFrom("name", "")));
            Assert.Throws<ArgumentException>(() => localizationService.SaveLanguage(CreateFrom("name", null)));
        }

        private static LanguageStateModel CreateFrom(string name, string code) =>
            new LanguageStateModel
            {
                Code = code,
                Name = name
            };
    }
}