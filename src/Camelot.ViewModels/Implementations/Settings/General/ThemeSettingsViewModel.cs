using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Factories.Interfaces;
using Camelot.ViewModels.Interfaces.Settings;
using DynamicData;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Settings.General
{
    public class ThemeSettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly IThemeService _themeService;
        private readonly IThemeViewModelFactory _themeViewModelFactory;

        private readonly ObservableCollection<ThemeViewModel> _themes;

        private ThemeViewModel _initialTheme;
        private bool _isActivated;

        public IEnumerable<ThemeViewModel> Themes => _themes;

        [Reactive]
        public ThemeViewModel CurrentTheme { get; set; }

        public bool IsChanged => CurrentTheme != _initialTheme;

        public ThemeSettingsViewModel(
            IThemeService themeService,
            IThemeViewModelFactory themeViewModelFactory)
        {
            _themeService = themeService;
            _themeViewModelFactory = themeViewModelFactory;

            _themes = new ObservableCollection<ThemeViewModel>();
        }

        public void Activate()
        {
            if (_isActivated)
            {
                return;
            }

            _isActivated = true;

            _themes.AddRange(_themeViewModelFactory.CreateAll());

            var selectedTheme = _themeService.GetCurrentTheme();
            _initialTheme = CurrentTheme = _themes.Single(vm => vm.Theme == selectedTheme);
        }

        public void SaveChanges()
        {
            var settings = CreateSettings();

            _themeService.SaveThemeSettings(settings);
        }

        private ThemeSettingsModel CreateSettings() =>
            new ThemeSettingsModel(CurrentTheme.Theme);
    }
}