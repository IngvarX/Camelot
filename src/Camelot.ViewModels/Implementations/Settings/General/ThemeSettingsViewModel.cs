using System.Collections.Generic;
using System.Collections.ObjectModel;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.Settings;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Settings.General
{
    public class ThemeSettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly IThemeService _themeService;

        private ObservableCollection<ThemeViewModel> _themes;

        private ThemeViewModel _initialThemeSettings;
        private bool _isActivated;

        public IEnumerable<ThemeViewModel> Themes => _themes;

        [Reactive]
        public ThemeViewModel CurrentThemeSettings { get; set; }

        public bool IsChanged => CurrentThemeSettings != _initialThemeSettings;

        public ThemeSettingsViewModel(
            IThemeService themeService)
        {
            _themeService = themeService;
        }

        public void Activate()
        {
            if (_isActivated)
            {
                return;
            }

            _isActivated = true;
        }

        public void SaveChanges()
        {
            var settings = CreateSettings();

            _themeService.SaveThemeSettings(settings);
        }

        private ThemeSettingsModel CreateSettings() =>
            new ThemeSettingsModel(CurrentThemeSettings.Theme);
    }
}