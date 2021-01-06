using System;
using Camelot.DataAccess.Models;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Services.Configuration;

namespace Camelot.Services
{
    public class ThemeService : IThemeService
    {
        private const string ThemeSettingsId = "ThemeSettings";

        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly DefaultThemeConfiguration _defaultThemeConfiguration;

        public ThemeService(
            IUnitOfWorkFactory unitOfWorkFactory,
            DefaultThemeConfiguration defaultThemeConfiguration)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _defaultThemeConfiguration = defaultThemeConfiguration;
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

        public Theme GetCurrentTheme()
        {
            var themeSettings = GetThemeSettings();

            return themeSettings?.SelectedTheme ?? _defaultThemeConfiguration.DefaultTheme;
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