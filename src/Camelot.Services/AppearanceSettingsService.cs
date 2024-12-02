using System;
using System.Collections.Generic;
using System.Linq;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services;

public class AppearanceSettingsService : IAppearanceSettingsService
{
    private const string SettingsId = "AppearanceSettings";
    private readonly AppearanceSettingsModel _default;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private AppearanceSettingsModel _cachedSettingsValue;
    public AppearanceSettingsService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _default = new AppearanceSettingsModel(false);
        GetAppearanceSettings();
    }

    public AppearanceSettingsModel GetAppearanceSettings()
    {
        if (_cachedSettingsValue == null)
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<AppearanceSettingsModel>();
            var dbModel = repository.GetById(SettingsId);
            if (dbModel != null)
                _cachedSettingsValue = dbModel;
            else
                _cachedSettingsValue = _default;
        }
        else
        {
            // we set value of _cachedValue in 'save',
            // so no need to read from the repository every time.
        }
        return _cachedSettingsValue;
    }


    public void SaveAppearanceSettings(AppearanceSettingsModel appearanceSettingsModel)
    {
        if (appearanceSettingsModel == null)
            throw new ArgumentNullException(nameof(appearanceSettingsModel));

        using var uow = _unitOfWorkFactory.Create();
        var repository = uow.GetRepository<AppearanceSettingsModel>();
        repository.Upsert(SettingsId, appearanceSettingsModel);
        _cachedSettingsValue = appearanceSettingsModel;
    }
}
