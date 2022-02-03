namespace Camelot.DataAccess.Repositories;

public interface IRepository<T>
{
    T GetById(string id);

    void Add(string id, T entity);

    void Update(string id, T entity);

    void Upsert(string id, T entity);

    void Remove(string id);
}