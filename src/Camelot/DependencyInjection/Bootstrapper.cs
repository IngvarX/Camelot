using Camelot.Configuration;
using Splat;

namespace Camelot.DependencyInjection;

public static class Bootstrapper
{
    public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver,
        DataAccessConfiguration dataAccessConfig)
    {
        EnvironmentServicesBootstrapper.RegisterEnvironmentServices(services, resolver);
        ConfigurationBootstrapper.RegisterConfiguration(services, resolver, dataAccessConfig);
        LoggingBootstrapper.RegisterLogging(services, resolver);
        AvaloniaServicesBootstrapper.RegisterAvaloniaServices(services);
        FileSystemWatcherServicesBootstrapper.RegisterFileSystemWatcherServices(services, resolver);
        DataAccessBootstrapper.RegisterDataAccess(services, resolver);
        ServicesBootstrapper.RegisterServices(services, resolver);
        ViewModelsBootstrapper.RegisterViewModels(services, resolver);
    }
}