using System;
using System.Collections.Generic;
using System.Linq;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services;

// Name of feature as "Quick search" is based on same name used by Total-Commander.
// Changing opacity of filtered items is based on muCommander.
public class QuickSearchService : IQuickSearchService
{
    private const string SettingsId = "QuickSearchSettings";

    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    private QuickSearchModel _cachedSettingsValue;
    private string _searchWord = string.Empty;
    private char _searchLetter = Char.MinValue;
    private int _selectedIndex = -1;

    public bool IsEnabled => _cachedSettingsValue.SelectedMode != QuickSearchMode.Disabled;

    public QuickSearchService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;

        GetQuickSearchSettings();
    }

    public QuickSearchModel GetQuickSearchSettings()
    {
        if (_cachedSettingsValue is not null)
        {
            return _cachedSettingsValue;
        }

        using var uow = _unitOfWorkFactory.Create();
        var repository = uow.GetRepository<QuickSearchModel>();
        var dbModel = repository.GetById(SettingsId) ?? new QuickSearchModel(QuickSearchMode.Disabled);

        return _cachedSettingsValue = dbModel;
    }

    public IReadOnlyList<QuickSearchNodeModel> FilterNodes(
        char symbol, bool isBackwardsDirectionEnabled, IReadOnlyList<string> nodes)
    {
        var lowercaseSymbol = Char.ToLower(symbol);
        switch (_cachedSettingsValue.SelectedMode)
        {
            case QuickSearchMode.Letter:
            {
                if (_searchLetter != lowercaseSymbol)
                {
                    _selectedIndex = -1;
                }

                _searchLetter = lowercaseSymbol;
                break;
            }
            case QuickSearchMode.Word:
            {
                _searchWord += lowercaseSymbol;
                break;
            }
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(_cachedSettingsValue.SelectedMode), _cachedSettingsValue.SelectedMode, null);
        }

        var result = nodes
            .Select(n => new QuickSearchNodeModel {Name = n, IsFiltered = CheckIfShouldIncludeInSearchResults(n)})
            .ToList();
        if (result.Any(n => n.IsFiltered))
        {
            SetSelectedItem(result, isBackwardsDirectionEnabled);
        }

        return result;
    }

    /// <summary>
    /// Set value of <see cref="QuickSearchNodeModel.Selected"/>
    /// which indicates to UI which item should be selected.
    /// </summary>
    private void SetSelectedItem(IReadOnlyList<QuickSearchNodeModel> files, bool isBackwardsDirectionEnabled)
    {
        _selectedIndex = ComputeNewSelectedIndex(files, _selectedIndex, isBackwardsDirectionEnabled);
        if (_selectedIndex >= 0)
        {
            var file = files[_selectedIndex];
            file.Selected = true;
        }
    }

    private static int ComputeNewSelectedIndex(
        IReadOnlyList<QuickSearchNodeModel> files,
        int selectedIndex,
        bool isBackwardsDirectionEnabled)
    {
        int start, jump;
        if (isBackwardsDirectionEnabled)
        {
            start = selectedIndex > -1 ? selectedIndex - 1 : 0;
            jump = -1;
        }
        else
        {
            start = selectedIndex > -1 ? selectedIndex + 1 : 0;
            jump = 1;
        }

        start = (start + files.Count) % files.Count;

        for (var i = start; i != start - jump; i = (i + jump + files.Count) % files.Count)
        {
            var file = files[i];
            if (file.IsFiltered)
            {
                return i;
            }
        }

        return -1;
    }

    private bool CheckIfShouldIncludeInSearchResults(string nodeName) =>
        _cachedSettingsValue.SelectedMode switch
        {
            QuickSearchMode.Letter => char.ToLower(nodeName[0]) == _searchLetter,
            QuickSearchMode.Word => nodeName.StartsWith(_searchWord, StringComparison.OrdinalIgnoreCase),
            _ => throw new ArgumentOutOfRangeException(nameof(_cachedSettingsValue.SelectedMode), _cachedSettingsValue, null)
        };

    public void ClearSearch()
    {
        if (!IsEnabled)
        {
            return;
        }

        _searchWord = string.Empty;
        _searchLetter = Char.MinValue;
        _selectedIndex = -1;
    }

    public void SaveQuickSearchSettings(QuickSearchModel quickSearchModel)
    {
        using var uow = _unitOfWorkFactory.Create();
        var repository = uow.GetRepository<QuickSearchModel>();

        repository.Upsert(SettingsId, quickSearchModel);
        _cachedSettingsValue = quickSearchModel;
    }
}
