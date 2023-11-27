using System.Collections.Generic;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.ViewModels.Interfaces.Settings;
using ReactiveUI.Fody.Helpers;

namespace Camelot.ViewModels.Implementations.Settings;

public class KeyboardSettingsViewModel : ViewModelBase, ISettingsViewModel
{
    private readonly IQuickSearchService _quickSearchService;
    private QuickSearchMode _initialMode;

    private bool _isActivated;

    [Reactive]
    public QuickSearchMode CurrentQuickSearchMode { get; set; }

    public IEnumerable<QuickSearchMode> QuickSearchModeOptions
    {
        get
        {
            return new []{
                QuickSearchMode.Disabled,
                QuickSearchMode.Letter,
                QuickSearchMode.Word };
        }
    }

    public bool IsChanged => _initialMode != CurrentQuickSearchMode;
    
    public KeyboardSettingsViewModel(
        IQuickSearchService quickSearchService)
    {
        _quickSearchService = quickSearchService;
    }

    public void Activate()
    {
        if (_isActivated)
        {
            return;
        }

        _isActivated = true;

        var model = _quickSearchService.GetQuickSearchSettings();
        _initialMode = model.SelectedMode;
        CurrentQuickSearchMode = _initialMode;
    }

    public void SaveChanges()
    {
        var model = new QuickSearchModel(CurrentQuickSearchMode);
        _quickSearchService.SaveQuickSearchSettings(model);
    }
}