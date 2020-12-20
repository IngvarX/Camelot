using Camelot.Avalonia.Implementations;
using Camelot.Avalonia.Interfaces;
using Camelot.DataAccess.Configuration;
using Camelot.DataAccess.LiteDb;
using Camelot.DataAccess.UnitOfWork;
using Splat;

namespace Camelot.DependencyInjection
{
    public static class DataAccessBootstrapper
    {
        public static void RegisterDataAccess(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IUnitOfWorkFactory>(() => new LiteDbUnitOfWorkFactory(
                resolver.GetRequiredService<DatabaseConfiguration>()
            ));
            services.RegisterLazySingleton<IClipboardService>(() => new ClipboardService());
            services.RegisterLazySingleton<IMainWindowProvider>(() => new MainWindowProvider());
        }
    }
}