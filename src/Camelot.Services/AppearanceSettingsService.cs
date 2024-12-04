using System;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;

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
            _cachedSettingsValue = dbModel ?? _default;
        }

        return _cachedSettingsValue;
    }


    public void SaveAppearanceSettings(AppearanceSettingsModel appearanceSettingsModel)
    {
        if (appearanceSettingsModel == null)
        {
            throw new ArgumentNullException(nameof(appearanceSettingsModel));
        }

        using var uow = _unitOfWorkFactory.Create();
        var repository = uow.GetRepository<AppearanceSettingsModel>();

        repository.Upsert(SettingsId, appearanceSettingsModel);
        _cachedSettingsValue = appearanceSettingsModel;
    }
}
