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

        public LanguageModel GetCurrentLanguage => CreateLanguageModel(Thread.CurrentThread.CurrentUICulture);

        public LanguageModel GetDefaultLanguage => CreateLanguageModel(CultureInfo.DefaultThreadCurrentUICulture);

        public IEnumerable<LanguageModel> GetAllLanguages => _availableLanguages.Value.Values;

        public void SetLanguage(LanguageModel languageModel)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(languageModel.Code);
        }

        private static Dictionary<string, LanguageModel> GetAvailableLanguages()
        {
            var languages = new Dictionary<string, LanguageModel>();

            foreach (var cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                if (cultureInfo.Equals(CultureInfo.InvariantCulture))
                {
                    continue;
                }

                var resourceSet = Resources.ResourceManager.GetResourceSet(cultureInfo, true, false);
                if (resourceSet == null)
                {
                    continue;
                }

                var languageModel = CreateLanguageModel(cultureInfo);
                languages.TryAdd(languageModel.Code, languageModel);
            }

            return languages;
        }

        private static LanguageModel CreateLanguageModel(CultureInfo cultureInfo)
            => new LanguageModel(cultureInfo.EnglishName, cultureInfo.NativeName.ToTitleCase(), cultureInfo.TwoLetterISOLanguageName);
    }
}