using Camelot.Services.Abstractions;
using Camelot.Services.FileSystemWatcher.Configuration;
using Camelot.Services.FileSystemWatcher.Implementations;
using Camelot.Services.FileSystemWatcher.Interfaces;
using Splat;

namespace Camelot.DependencyInjection;

public static class FileSystemWatcherServicesBootstrapper
{
    public static void RegisterFileSystemWatcherServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton<IFileSystemWatcherFactory>(() => new FileSystemWatcherFactory(
            resolver.GetRequiredService<IPathService>(),
            resolver.GetRequiredService<FileSystemWatcherConfiguration>()
        ));
    }
}