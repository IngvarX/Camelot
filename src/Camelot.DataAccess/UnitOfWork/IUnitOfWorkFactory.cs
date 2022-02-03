namespace Camelot.DataAccess.UnitOfWork;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create();
}