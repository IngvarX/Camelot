using Camelot.ViewModels.Interfaces.Settings;

namespace Camelot.ViewModels.Implementations.Settings
{
    public class GeneralSettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        public ISettingsViewModel LanguageSettingsViewModel { get; }

        public bool IsChanged => LanguageSettingsViewModel.IsChanged;

        public GeneralSettingsViewModel(ISettingsViewModel languageSettingsViewModel)
        {
            LanguageSettingsViewModel = languageSettingsViewModel;
        }

        public void Activate()
        {
            LanguageSettingsViewModel.Activate();
        }

        public void SaveChanges()
        {
            LanguageSettingsViewModel.SaveChanges();
        }
    }
}
