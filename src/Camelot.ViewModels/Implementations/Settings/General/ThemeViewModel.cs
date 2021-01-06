using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.ViewModels.Implementations.Settings.General
{
    public class ThemeViewModel : ViewModelBase
    {
        public Theme Theme { get; }

        public string ThemeName { get; }

        public ThemeViewModel(Theme theme, string themeName)
        {
            Theme = theme;
            ThemeName = themeName;
        }
    }
}