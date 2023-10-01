using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions;

public interface IIconsSettingsService
{
    IconsSettingsModel GetIconsSettings();

    void SaveIconsSettings(IconsSettingsModel iconsSettingsModel);
}