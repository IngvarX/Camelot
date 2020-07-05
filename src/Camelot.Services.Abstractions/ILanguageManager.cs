using System.Collections.Generic;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface ILanguageManager
    {
        LanguageModel GetCurrentLanguage { get; }

        LanguageModel GetDefaultLanguage { get; }

        IEnumerable<LanguageModel> GetAllLanguages { get; }

        void SetLanguage(LanguageModel languageModel);
    }
}
