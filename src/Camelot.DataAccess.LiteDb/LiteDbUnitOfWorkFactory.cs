using Camelot.DataAccess.UnitOfWork;
using LiteDB;

namespace Camelot.DataAccess.LiteDb
{
    public class LiteDbUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private const string DatabaseName = "Camelot.db";
        
        public IUnitOfWork Create()
        {
            var database = new LiteDatabase(DatabaseName);
            
            return new LiteDbUnitOfWork(database);
        }
    }
}