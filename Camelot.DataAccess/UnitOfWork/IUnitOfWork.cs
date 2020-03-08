using System;
using System.Threading.Tasks;
using Camelot.DataAccess.Repositories;

namespace Camelot.DataAccess.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;

        Task SaveChangesAsync();
    }
}