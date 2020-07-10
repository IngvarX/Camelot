using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.Settings;
using ReactiveUI;

namespace Camelot.ViewModels.Implementations.Settings.General
{
    public class LanguageSettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageManager _languageManager;

        private LanguageModel _currentLanguage;
        private LanguageModel _initialLanguage;
        private readonly ObservableCollection<LanguageModel> _languages;
        private bool _isActivated;

        public LanguageModel CurrentLanguage
        {
            get => _currentLanguage;
            set => this.RaiseAndSetIfChanged(ref _currentLanguage, value);
        }

        public IEnumerable<LanguageModel> Languages => _languages;

        public LanguageSettingsViewModel(
            ILocalizationService localizationService,
            ILanguageManager languageManager)
        {
            _localizationService = localizationService;
            _languageManager = languageManager;

            _languages = SetupLanguages();
        }

        public bool IsChanged => _initialLanguage != CurrentLanguage;

        public void Activate()
        {
            if (_isActivated)
            {
                return;
            }

            _isActivated = true;

            var savedLanguage = _localizationService.GetSavedLanguage();
            var currentLanguage = _languageManager.CurrentLanguage;

            var languageCode = savedLanguage != null ? savedLanguage.Code : currentLanguage.Code;
            CurrentLanguage = _initialLanguage = GetLanguageOrDefault(languageCode);
        }

        public void SaveChanges()
        {
            _languageManager.SetLanguage(CurrentLanguage);
            _localizationService.SaveLanguage(CurrentLanguage);
        }

        private LanguageModel GetLanguageOrDefault(string languageCode)
            => Languages.SingleOrDefault(l => l.Code == languageCode) ?? _languageManager.DefaultLanguage;

        private ObservableCollection<LanguageModel> SetupLanguages()
            => new ObservableCollection<LanguageModel>(_languageManager.GetAllLanguages);
    }
}
