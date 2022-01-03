using Camelot.Services.Environment.Enums;
using Camelot.Services.Environment.Implementations;
using Camelot.Services.Environment.Interfaces;
using Splat;

namespace Camelot.DependencyInjection;

public static class EnvironmentServicesBootstrapper
{
    public static void RegisterEnvironmentServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        RegisterCommonServices(services);
        RegisterPlatformSpecificServices(services, resolver);
    }

    private static void RegisterCommonServices(IMutableDependencyResolver services)
    {
        services.RegisterLazySingleton<IEnvironmentService>(() => new EnvironmentService());
        services.RegisterLazySingleton<IProcessService>(() => new ProcessService());
        services.RegisterLazySingleton<IEnvironmentFileService>(() => new EnvironmentFileService());
        services.RegisterLazySingleton<IEnvironmentDirectoryService>(() => new EnvironmentDirectoryService());
        services.RegisterLazySingleton<IEnvironmentDriveService>(() => new EnvironmentDriveService());
        services.RegisterLazySingleton<IEnvironmentPathService>(() => new EnvironmentPathService());
        services.RegisterLazySingleton<IRegexService>(() => new RegexService());
        services.Register<IPlatformService>(() => new PlatformService());
    }

    private static void RegisterPlatformSpecificServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        var platformService = resolver.GetRequiredService<IPlatformService>();
        var platform = platformService.GetPlatform();

        if (platform is Platform.Windows)
        {
            RegisterWindowsServices(services, resolver);
        }
    }

    private static void RegisterWindowsServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton<IEnvironmentFileService>(() =>
            new EnvironmentFileServiceWindowsDecorator(
                new EnvironmentFileService()
            )
        );
        services.RegisterLazySingleton<IEnvironmentDirectoryService>(() =>
            new EnvironmentDirectoryServiceWindowsDecorator(
                new EnvironmentDirectoryService()
            )
        );
    }
}