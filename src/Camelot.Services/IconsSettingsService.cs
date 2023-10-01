using System;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Interfaces;

namespace Camelot.Services;

public class IconsSettingsService : IIconsSettingsService
{
    private const string SettingsId = "IconsSettings";
    private readonly IconsSettingsModel _default;
    private readonly IconsSettingsModel _builtin;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly Platform _platform;
    private IconsSettingsModel _cachedValue = null;

    public IconsSettingsService(IUnitOfWorkFactory unitOfWorkFactory,
        IPlatformService platformService)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _platform = platformService.GetPlatform();
        _default = new IconsSettingsModel(IconsType.Shell);
        _builtin = new IconsSettingsModel(IconsType.Builtin);
    }

    
    public IconsSettingsModel GetIconsSettings()
    {
        if (_platform != Platform.Windows)
            return _builtin;

        if (_cachedValue == null)
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<IconsSettingsModel>();
            var dbModel = repository.GetById(SettingsId);
            if (dbModel != null)
                _cachedValue = dbModel;
            else
                _cachedValue = _default;
        }
        else
        {
            // we set value of _cachedValue in 'save',
            // so no need to read from the repository every time.
        }
        return _cachedValue;
    }

    public void SaveIconsSettings(IconsSettingsModel iconsSettingsModel)
    {
        if (_platform != Platform.Windows)
            return;

        if (iconsSettingsModel is null)
        {
            throw new ArgumentNullException(nameof(iconsSettingsModel));
        }

        using var uow = _unitOfWorkFactory.Create();
        var repository = uow.GetRepository<IconsSettingsModel>();
        repository.Upsert(SettingsId, iconsSettingsModel);
        _cachedValue = iconsSettingsModel;
    }
}