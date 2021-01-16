using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Camelot.DependencyInjection;
using Camelot.Services.Abstractions;
using Camelot.Services.Abstractions.Models.Enums;
using Camelot.Styles.Themes;
using Camelot.ViewModels.Implementations;
using Camelot.Views;
using Splat;
using Application = Avalonia.Application;

namespace Camelot
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            LoadSettings();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = GetRequiredService<MainWindowViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void LoadSettings()
        {
            LoadTheme();
            LoadLanguage();
        }

        private void LoadTheme()
        {
            var themeService = GetRequiredService<IThemeService>();
            var selectedTheme = themeService.GetCurrentTheme();

            switch (selectedTheme)
            {
                case Theme.Dark:
                    Styles.Add(new DarkTheme());
                    break;
                case Theme.Light:
                    Styles.Add(new LightTheme());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(selectedTheme), selectedTheme, null);
            }
        }

        private static void LoadLanguage()
        {
            var localizationService = GetRequiredService<ILocalizationService>();
            if (localizationService.GetSavedLanguage() is { } savedLanguage)
            {
                var languageManager = GetRequiredService<ILanguageManager>();

                languageManager.SetLanguage(savedLanguage.Code);
            }
        }

        private static T GetRequiredService<T>() => Locator.Current.GetRequiredService<T>();
    }
}