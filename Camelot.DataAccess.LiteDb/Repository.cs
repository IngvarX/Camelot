using System.Threading.Tasks;
using Camelot.DataAccess.Repositories;
using LiteDB;

namespace Camelot.DataAccess.LiteDb
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ILiteCollection<T> _collection;

        public Repository(ILiteCollection<T> collection)
        {
            _collection = collection;
        }
        
        public Task<T> GetByIdAsync(string id)
        {
            var entity = _collection.FindById(id);

            return Task.FromResult(entity);
        }

        public void Add(T entity)
        {
            _collection.Insert(entity);
        }

        public void Update(T entity)
        {
            _collection.Update(entity);
        }

        public void Remove(string key)
        {
            _collection.Delete(key);
        }
    }
}