using System.Collections.Generic;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface ILanguageManager
    {
        LanguageModel CurrentLanguage { get; }

        LanguageModel DefaultLanguage { get; }

        IEnumerable<LanguageModel> GetAllLanguages { get; }

        void SetLanguage(string langCode);

        void SetLanguage(LanguageModel languageModel);
    }
}
