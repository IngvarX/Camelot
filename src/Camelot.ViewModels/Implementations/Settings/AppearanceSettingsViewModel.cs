using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.ViewModels.Interfaces.Settings;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Settings;

public class AppearanceSettingsViewModel : ViewModelBase, ISettingsViewModel
{
    private readonly IAppearanceSettingsService _appearanceSettingService;
    private bool _initialShowKeyboardShortcuts;

    private bool _isActivated;

    [Reactive]
    public bool ShowKeyboardShortcuts { get; set; }


    public bool IsChanged => _initialShowKeyboardShortcuts != ShowKeyboardShortcuts;
    
    public AppearanceSettingsViewModel(
        IAppearanceSettingsService appearanceSettingService)
    {
        _appearanceSettingService = appearanceSettingService;
    }

    public void Activate()
    {
        if (_isActivated)
        {
            return;
        }

        _isActivated = true;

        var model = _appearanceSettingService.GetAppearanceSettings();
        _initialShowKeyboardShortcuts = model.ShowKeyboardShortcuts;
        ShowKeyboardShortcuts = _initialShowKeyboardShortcuts;
    }

    public void SaveChanges()
    {
        var model = new AppearanceSettingsModel(ShowKeyboardShortcuts);
        _appearanceSettingService.SaveAppearanceSettings(model);
    }
}