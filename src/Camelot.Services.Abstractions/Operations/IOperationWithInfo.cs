using Camelot.Services.Abstractions.Models.Operations;

namespace Camelot.Services.Abstractions.Operations;

public interface IOperationWithInfo
{
    OperationInfo Info { get; }
}