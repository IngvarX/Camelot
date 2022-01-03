using System.Collections.Generic;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations.Models;

public class OperationGroup
{
    public IReadOnlyList<IInternalOperation> Operations { get; }

    public OperationGroup(
        IReadOnlyList<IInternalOperation> operations)
    {
        Operations = operations;
    }
}