using Camelot.Services.Abstractions.Models.State;

namespace Camelot.Services.Abstractions
{
    public interface ILocalizationService
    {
        LanguageStateModel GetSavedLanguage();

        void SaveLanguage(LanguageStateModel language);
    }
}
