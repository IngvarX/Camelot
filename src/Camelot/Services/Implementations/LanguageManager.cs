using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Camelot.Extensions;
using Camelot.Properties;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Implementations
{
    public class LanguageManager : ILanguageManager
    {
        private readonly Lazy<Dictionary<string, LanguageModel>> _availableLanguages;

        public LanguageModel DefaultLanguage { get; }

        public LanguageModel CurrentLanguage => CreateLanguageModel(Thread.CurrentThread.CurrentUICulture);

        public IEnumerable<LanguageModel> AllLanguages => _availableLanguages.Value.Values;

        public LanguageManager()
        {
            _availableLanguages = new Lazy<Dictionary<string, LanguageModel>>(GetAvailableLanguages);

            DefaultLanguage = CreateLanguageModel(CultureInfo.GetCultureInfo("en"));
        }

        public void SetLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                throw new ArgumentException($"{nameof(languageCode)} can't be empty.");
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(languageCode);
        }

        public void SetLanguage(LanguageModel languageModel) => SetLanguage(languageModel.Code);

        private Dictionary<string, LanguageModel> GetAvailableLanguages()
        {
            var languages = new Dictionary<string, LanguageModel>
            {
                { DefaultLanguage.Code, DefaultLanguage }
            };

            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (cultureInfo.Equals(CultureInfo.InvariantCulture))
                {
                    continue;
                }

                var resourceSet = Resources.ResourceManager.GetResourceSet(cultureInfo, true, false);
                if (resourceSet is null)
                {
                    continue;
                }

                var languageModel = CreateLanguageModel(cultureInfo);
                languages.TryAdd(languageModel.Code, languageModel);
            }

            return languages;
        }

        private LanguageModel CreateLanguageModel(CultureInfo cultureInfo) =>
            cultureInfo is null
                ? DefaultLanguage
                : new LanguageModel(cultureInfo.EnglishName, cultureInfo.NativeName.ToTitleCase(),
                    cultureInfo.TwoLetterISOLanguageName);
    }
}