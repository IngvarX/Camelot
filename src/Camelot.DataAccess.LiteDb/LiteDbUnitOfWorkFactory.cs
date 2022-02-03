using System.IO;
using Camelot.DataAccess.Configuration;
using Camelot.DataAccess.UnitOfWork;
using LiteDB;

namespace Camelot.DataAccess.LiteDb;

public class LiteDbUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly DatabaseConfiguration _databaseConfiguration;

    public LiteDbUnitOfWorkFactory(DatabaseConfiguration databaseConfiguration)
    {
        _databaseConfiguration = databaseConfiguration;
    }

    public IUnitOfWork Create()
    {
        var database = _databaseConfiguration.UseInMemoryDatabase
            ? CreateInMemoryDatabase()
            : CreateDatabaseFromConnectionString();

        return new LiteDbUnitOfWork(database);
    }

    private static LiteDatabase CreateInMemoryDatabase() => new(new MemoryStream());

    private LiteDatabase CreateDatabaseFromConnectionString() =>
        new(_databaseConfiguration.ConnectionString);
}