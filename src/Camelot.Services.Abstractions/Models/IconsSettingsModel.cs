using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models;

public class IconsSettingsModel
{
    public IconsType SelectedIconsType { get; }

    public IconsSettingsModel(IconsType selectedIconsType)
    {
        SelectedIconsType = selectedIconsType;
    }
}