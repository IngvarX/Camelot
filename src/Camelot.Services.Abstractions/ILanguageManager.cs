using System.Collections.Generic;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface ILanguageManager
    {
        LanguageModel CurrentLanguage { get; }

        LanguageModel DefaultLanguage { get; }

        IEnumerable<LanguageModel> AllLanguages { get; }

        void SetLanguage(string languageCode);

        void SetLanguage(LanguageModel languageModel);
    }
}
