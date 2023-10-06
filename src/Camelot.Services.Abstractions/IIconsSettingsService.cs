using Camelot.Services.Abstractions.Models;

namespace Camelot.Services.Abstractions;

public interface IIconsSettingsService
{
    IconsSettingsModel GetIconsSettings();

    void SaveIconsSettings(IconsSettingsModel iconsSettingsModel);
}