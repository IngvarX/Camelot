using Camelot.DataAccess.Models;
using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface ILocalizationService
    {
        Language GetSavedLanguage();

        void SaveLanguage(LanguageModel language);
    }
}
