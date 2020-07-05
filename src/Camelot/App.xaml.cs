using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
                    DataContext = Locator.Current.GetService<MainWindowViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void LoadTheme()
        {
            // TODO: load themes on start from db
            Styles.Add(new DarkTheme());
        }

        private void LoadLanguage()
        {
            var localizationService = Locator.Current.GetService<ILocalizationService>();
            var languageManager = Locator.Current.GetService<ILanguageManager>();

            var savedLanguage = localizationService.GetSavedLanguage();
            if (savedLanguage != null)
            {
                languageManager.SetLanguage(savedLanguage.Code);
            }
        }
    }
}