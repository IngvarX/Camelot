using System.Threading.Tasks;
using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations
{
    public interface IOperation : IInternalOperation
    {
        OperationInfo Info { get; }

        Task CancelAsync();
    }
}