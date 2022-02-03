using Camelot.DataAccess.Configuration;
using Camelot.DataAccess.LiteDb;
using Camelot.DataAccess.UnitOfWork;
using Splat;

namespace Camelot.DependencyInjection;

public static class DataAccessBootstrapper
{
    public static void RegisterDataAccess(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
    {
        services.RegisterLazySingleton<IUnitOfWorkFactory>(() => new LiteDbUnitOfWorkFactory(
            resolver.GetRequiredService<DatabaseConfiguration>()
        ));
    }
}