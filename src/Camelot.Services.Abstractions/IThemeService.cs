using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions
{
    public interface IThemeService
    {
        ThemeSettingsModel GetThemeSettings();

        void SaveThemeSettings(ThemeSettingsModel themeSettingsModel);
    }
}