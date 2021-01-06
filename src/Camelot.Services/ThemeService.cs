using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services
{
    public class ThemeService : IThemeService
    {
        private const string ThemeSettingsId = "ThemeSettings";

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public ThemeService(IUnitOfWorkFactory unitOfWorkFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public ThemeSettingsModel GetThemeSettings()
        {
            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<ThemeSettings>();
            var dbModel = repository.GetById(ThemeSettingsId);

            return CreateFrom(dbModel);
        }

        public void SaveThemeSettings(ThemeSettingsModel themeSettingsModel)
        {
            if (themeSettingsModel is null)
            {
                throw new ArgumentNullException(nameof(themeSettingsModel));
            }

            using var uow = _unitOfWorkFactory.Create();
            var repository = uow.GetRepository<ThemeSettings>();

            var themeSettings = CreateFrom(themeSettingsModel);

            repository.Upsert(ThemeSettingsId, themeSettings);
        }

        private static ThemeSettingsModel CreateFrom(ThemeSettings dbModel) =>
            dbModel is null ? null : new ThemeSettingsModel((Theme) dbModel.SelectedTheme);

        private static ThemeSettings CreateFrom(ThemeSettingsModel settingsModel) =>
            new ThemeSettings
            {
                SelectedTheme = (int) settingsModel.SelectedTheme
            };
    }
}