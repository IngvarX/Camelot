using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions;

public interface IAppearanceSettingsService
{
    AppearanceSettingsModel GetAppearanceSettings();

    void SaveAppearanceSettings(AppearanceSettingsModel appearanceSettingsModel);
}