using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.ReactiveUI;
using Avalonia.Threading;
using Camelot.Configuration;
using Camelot.DependencyInjection;
using Camelot.Views;
using Splat;

namespace Camelot.Ui.Tests.Common;

public static class AvaloniaApp
{
    public static void RegisterDependencies()
    {
        var config = new DataAccessConfiguration
        {
            UseInMemoryDatabase = true
        };

        Bootstrapper.Register(Locator.CurrentMutable, Locator.Current, config);
    }

    public static void Stop()
    {
        var app = GetApp();
        if (app is IDisposable disposable)
        {
            Dispatcher.UIThread.Post(disposable.Dispose);
        }

        Dispatcher.UIThread.Post(() => app.Shutdown());
    }

    public static MainWindow GetMainWindow() => (MainWindow) GetApp().MainWindow;

    public static IClassicDesktopStyleApplicationLifetime GetApp() =>
        (IClassicDesktopStyleApplicationLifetime) Application.Current.ApplicationLifetime;

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
            .UseHeadless();
}