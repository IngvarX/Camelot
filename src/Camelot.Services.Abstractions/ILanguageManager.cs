using System.Collections.Generic;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface ILanguageManager
    {
        LanguageModel GetLanguage();

        void SetLanguage(LanguageModel languageModel);

        IEnumerable<LanguageModel> GetAllLanguages();
    }
}
