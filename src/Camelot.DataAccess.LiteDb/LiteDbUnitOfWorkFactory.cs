using Camelot.DataAccess.Configuration;
using Camelot.DataAccess.UnitOfWork;
using LiteDB;

namespace Camelot.DataAccess.LiteDb
{
    public class LiteDbUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly DatabaseConfiguration _databaseConfiguration;

        public LiteDbUnitOfWorkFactory(DatabaseConfiguration databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration;
        }
        
        public IUnitOfWork Create()
        {
            var database = new LiteDatabase(_databaseConfiguration.ConnectionString);
            
            return new LiteDbUnitOfWork(database);
        }
    }
}