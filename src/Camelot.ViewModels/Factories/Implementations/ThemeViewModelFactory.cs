using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Configuration;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Implementations.Settings.General;
using Camelot.ViewModels.Services.Interfaces;

namespace Camelot.ViewModels.Factories.Implementations
{
    public class ThemeViewModelFactory : IThemeViewModelFactory
    {
        private readonly IResourceProvider _resourceProvider;
        private readonly ThemesConfiguration _themesConfiguration;

        public ThemeViewModelFactory(
            IResourceProvider resourceProvider,
            ThemesConfiguration themesConfiguration)
        {
            _resourceProvider = resourceProvider;
            _themesConfiguration = themesConfiguration;
        }

        public ThemeViewModel Create(Theme theme)
        {
            var themeResourceName = _themesConfiguration.ThemeToResourceMapping[theme];
            var themeTranslatedName = _resourceProvider.GetResourceByName(themeResourceName);

            return new ThemeViewModel(theme, themeTranslatedName);
        }
    }
}