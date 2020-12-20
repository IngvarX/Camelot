using Camelot.FileSystemWatcher.Configuration;
using Camelot.FileSystemWatcher.Implementations;
using Camelot.FileSystemWatcher.Interfaces;
using Camelot.Services.Abstractions;
using Splat;

namespace Camelot.DependencyInjection
{
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
}