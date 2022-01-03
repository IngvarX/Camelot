using Camelot.Avalonia.Implementations;
using Camelot.Avalonia.Interfaces;
using Splat;

namespace Camelot.DependencyInjection;

public static class AvaloniaServicesBootstrapper
{
    public static void RegisterAvaloniaServices(IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<IApplicationCloser>(() => new ApplicationCloser());
        services.RegisterLazySingleton<IApplicationDispatcher>(() => new AvaloniaDispatcher());
        services.RegisterLazySingleton<IApplicationVersionProvider>(() => new ApplicationVersionProvider());
        services.RegisterLazySingleton<IClipboardService>(() => new ClipboardService());
        services.RegisterLazySingleton<IMainWindowProvider>(() => new MainWindowProvider());
    }
}