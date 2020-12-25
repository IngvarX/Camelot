using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Camelot.DependencyInjection;
using Camelot.Services.Abstractions;
using Camelot.Styles.Themes;
using Camelot.ViewModels.Implementations;
using Camelot.Views;
using Splat;

namespace Camelot
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            LoadTheme();
            LoadLanguage();
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

        private void LoadTheme()
        {
            // TODO: load themes on start from db
            Styles.Add(new LightTheme());
        }

        private static void LoadLanguage()
        {
            var localizationService = GetRequiredService<ILocalizationService>();
            var languageManager = GetRequiredService<ILanguageManager>();

            if (localizationService.GetSavedLanguage() is { } savedLanguage)
            {
                languageManager.SetLanguage(savedLanguage.Code);
            }
        }

        private static T GetRequiredService<T>() => Locator.Current.GetRequiredService<T>();
    }
}