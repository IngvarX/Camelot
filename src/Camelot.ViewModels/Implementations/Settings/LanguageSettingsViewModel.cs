using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.Settings;
using DynamicData;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Settings
{
    public class LanguageSettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageManager _languageManager;

        private LanguageModel _currentLanguage;
        private readonly ObservableCollection<LanguageModel> _allLanguages;
        private bool _isActivated;

        public LanguageModel CurrentLanguage
        {
            get => _currentLanguage;
            set => this.RaiseAndSetIfChanged(ref _currentLanguage, value);
        }

        public IEnumerable<LanguageModel> AllLanguages => _allLanguages;

        public LanguageSettingsViewModel(
            ILocalizationService localizationService, 
            ILanguageManager languageManager)
        {
            _localizationService = localizationService;
            _languageManager = languageManager;

            _allLanguages = SetupLanguages();
        }

        public bool IsChanged => _currentLanguage != CurrentLanguage;

        public void Activate()
        {
            if (_isActivated)
            {
                return;
            }

            _isActivated = true;

            var savedLanguage = _localizationService.GetSavedLanguage();
            var currentLanguage = _languageManager.GetLanguage();

            if (savedLanguage != null)
            {

            }
        }

        public void SaveChanges()
        {
        }

        private ObservableCollection<LanguageModel> SetupLanguages() 
            => new ObservableCollection<LanguageModel>(_languageManager.GetAllLanguages());
    }
}
