using Camelot.DataAccess.Repositories;
using Camelot.DataAccess.UnitOfWork;
using LiteDB;

namespace Camelot.DataAccess.LiteDb;

public class LiteDbUnitOfWork : IUnitOfWork
{
    private readonly LiteDatabase _database;

    public LiteDbUnitOfWork(LiteDatabase database)
    {
        _database = database;
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        var collection = _database.GetCollection<T>();

        return new Repository<T>(collection);
    }

    public void SaveChanges() => _database.Commit();

    public void Dispose() => _database.Dispose();
}