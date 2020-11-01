using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services
{
    public class LocalizationService : ILocalizationService
    {
        private const string LanguageSettingsId = "LanguageSettings";

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public LocalizationService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public LanguageModel GetSavedLanguage()
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<Language>();
            var dbModel = repository.GetById(LanguageSettingsId);

            return CreateFrom(dbModel);
        }

        public void SaveLanguage(LanguageModel languageModel)
        {
            if (languageModel is null)
            {
                throw new ArgumentNullException(nameof(languageModel));
            }

            if (string.IsNullOrEmpty(languageModel.Name))
            {
                throw new ArgumentException($"{nameof(languageModel.Name)} can't be empty.");
            }

            if (string.IsNullOrEmpty(languageModel.Code))
            {
                throw new ArgumentException($"{nameof(languageModel.Code)} can't be empty.");
            }

            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<Language>();

            var language = CreateFrom(languageModel);

            repository.Upsert(LanguageSettingsId, language);
        }

        private static LanguageModel CreateFrom(Language model) =>
            new LanguageModel
            {
                Code = model.Code,
                Name = model.Name
            };

        private static Language CreateFrom(LanguageModel model) =>
            new Language
            {
                Code = model.Code,
                Name = model.Name
            };
    }
}
