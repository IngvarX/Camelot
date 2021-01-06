using System.Linq;
using Camelot.Extensions;
using Camelot.ViewModels.Interfaces.Settings;

namespace Camelot.ViewModels.Implementations.Settings
{
    public class GeneralSettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ISettingsViewModel[] _settingsViewModels;

        public ISettingsViewModel LanguageSettingsViewModel { get; }

        public ISettingsViewModel ThemeViewModel { get; }

        public bool IsChanged => _settingsViewModels.Any(vm => vm.IsChanged);

        public GeneralSettingsViewModel(
            ISettingsViewModel languageSettingsViewModel,
            ISettingsViewModel themeViewModel)
        {
            LanguageSettingsViewModel = languageSettingsViewModel;
            ThemeViewModel = themeViewModel;

            _settingsViewModels = new[]
            {
                languageSettingsViewModel,
                themeViewModel
            };
        }

        public void Activate() => _settingsViewModels.ForEach(vm => vm.Activate());

        public void SaveChanges() => _settingsViewModels.ForEach(vm => vm.SaveChanges());
    }
}
