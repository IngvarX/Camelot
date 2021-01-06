using System.Collections.Generic;
using System.Linq;
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
        private readonly ThemesNamesConfiguration _themesNamesConfiguration;

        public ThemeViewModelFactory(
            IResourceProvider resourceProvider,
            ThemesNamesConfiguration themesNamesConfiguration)
        {
            _resourceProvider = resourceProvider;
            _themesNamesConfiguration = themesNamesConfiguration;
        }

        public IReadOnlyList<ThemeViewModel> CreateAll() =>
            _themesNamesConfiguration
                .ThemeToResourceMapping
                .Keys
                .Select(Create)
                .ToArray();

        private ThemeViewModel Create(Theme theme)
        {
            var themeResourceName = _themesNamesConfiguration.ThemeToResourceMapping[theme];
            var themeTranslatedName = _resourceProvider.GetResourceByName(themeResourceName);

            return new ThemeViewModel(theme, themeTranslatedName);
        }
    }
}