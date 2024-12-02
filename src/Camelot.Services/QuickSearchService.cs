using System;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Extensions;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services;

public class QuickSearchService : IQuickSearchService
{
    private const string SettingsId = "QuickSearchSettings";

    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    private QuickSearchModel _cachedSettingsValue;

    public bool IsEnabled => _cachedSettingsValue.SelectedMode != QuickSearchMode.Disabled;

    public event EventHandler<EventArgs> QuickSearchModeChanged;

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

    public void SaveQuickSearchSettings(QuickSearchModel quickSearchModel)
    {
        using var uow = _unitOfWorkFactory.Create();
        var repository = uow.GetRepository<QuickSearchModel>();

        repository.Upsert(SettingsId, quickSearchModel);
        _cachedSettingsValue = quickSearchModel;

        RaiseQuickSearchModeChangedEvent();
    }

    private void RaiseQuickSearchModeChangedEvent() => QuickSearchModeChanged.Raise(this, EventArgs.Empty);
}
