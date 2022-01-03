using System;
using Camelot.DataAccess.Repositories;

namespace Camelot.DataAccess.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> GetRepository<T>() where T : class;

    void SaveChanges();
}