using System.Threading.Tasks;

namespace Camelot.DataAccess.Repositories
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(string id);
        
        void Add(T entity);

        void Update(T entity);

        void Remove(string key);
    }
}