using Camelot.Services.Environment.Implementations;
using Camelot.Services.Environment.Interfaces;
using Splat;

namespace Camelot.DependencyInjection
{
    public static class EnvironmentServicesBootstrapper
    {
        public static void RegisterEnvironmentServices(IMutableDependencyResolver services)
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
    }
}