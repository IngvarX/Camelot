using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface ILocalizationService
    {
        LanguageModel GetSavedLanguage();

        void SaveLanguage(LanguageModel language);
    }
}
