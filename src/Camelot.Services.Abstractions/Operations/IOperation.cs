namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperation : ISuspendableOperation, IStatefulOperation, IOperationWithProgress
    {

    }
}