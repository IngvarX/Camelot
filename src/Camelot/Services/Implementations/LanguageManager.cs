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
        private readonly Lazy<Dictionary<string, LanguageModel>> _availableLanguages =
            new Lazy<Dictionary<string, LanguageModel>>(GetAvailableLanguages);

        private static readonly LanguageModel _defaultLanguage = CreateLanguageModel(CultureInfo.GetCultureInfo("en"));

        public LanguageModel DefaultLanguage => _defaultLanguage;

        public LanguageModel CurrentLanguage => CreateLanguageModel(Thread.CurrentThread.CurrentUICulture);

        public IEnumerable<LanguageModel> AllLanguages => _availableLanguages.Value.Values;

        public void SetLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                throw new ArgumentException($"{nameof(languageCode)} can't be empty.");
            }

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(languageCode);
        }

        public void SetLanguage(LanguageModel languageModel) =>
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(languageModel.Code);

        private static Dictionary<string, LanguageModel> GetAvailableLanguages()
        {
            var languages = new Dictionary<string, LanguageModel>
            {
                { _defaultLanguage.Code, _defaultLanguage }
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

        private static LanguageModel CreateLanguageModel(CultureInfo cultureInfo) =>
            cultureInfo is null
                ? _defaultLanguage
                : new LanguageModel(cultureInfo.EnglishName, cultureInfo.NativeName.ToTitleCase(),
                    cultureInfo.TwoLetterISOLanguageName);
    }
}