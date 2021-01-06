using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models
{
    public class ThemeSettingsModel
    {
        public Theme SelectedTheme { get; }

        public ThemeSettingsModel(Theme selectedTheme)
        {
            SelectedTheme = selectedTheme;
        }
    }
}