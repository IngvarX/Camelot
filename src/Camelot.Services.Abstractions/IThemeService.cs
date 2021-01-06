using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions
{
    public interface IThemeService
    {
        ThemeSettingsModel GetThemeSettings();

        void SaveThemeSettings(ThemeSettingsModel themeSettingsModel);

        Theme GetCurrentTheme();
    }
}