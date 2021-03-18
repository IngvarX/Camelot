using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.ReactiveUI;
using Camelot.DependencyInjection;
using Splat;

namespace Camelot.Ui.Tests
{
    public static class AvaloniaApp
    {
        public static void RegisterDependencies() =>
            Bootstrapper.Register(Locator.CurrentMutable, Locator.Current);

        public static Window GetMainWindow() => GetApp().MainWindow;

        public static IClassicDesktopStyleApplicationLifetime GetApp() =>
            (IClassicDesktopStyleApplicationLifetime) Application.Current.ApplicationLifetime;

        public static void Stop()
        {
            var app = GetApp();
            if (app is IDisposable disposable)
            {
                disposable.Dispose();
            }

            app.Shutdown();
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            var type = typeof(AppBuilderBase<AppBuilder>);
            var field = type.GetField("s_setupWasAlreadyCalled", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, false);

            return AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .UseHeadless();
        }
    }
}