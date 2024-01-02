using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions;

public interface IAppearanceSettingsService
{
    AppearanceSettingsModel GetAppearanceSettings();

    void SaveAppearanceSettings(AppearanceSettingsModel appearanceSettingsModel);
}