using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.State;

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

        public LanguageStateModel GetSavedLanguage()
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<Language>();
            var dbModel = repository.GetById(LanguageSettingsId);

            return CreateFrom(dbModel);
        }

        public void SaveLanguage(LanguageStateModel languageModel)
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

        private static LanguageStateModel CreateFrom(Language model) =>
            model is null
                ? null
                : new LanguageStateModel
                {
                    Code = model.Code,
                    Name = model.Name
                };

        private static Language CreateFrom(LanguageStateModel model) =>
            new Language
            {
                Code = model.Code,
                Name = model.Name
            };
    }
}
