using System.Collections.Generic;
using System.Linq;
using Camelot.Services.Abstractions.Operations;

namespace Camelot.Services.Operations.Models
{
    public class OperationGroup
    {
        public IReadOnlyList<IInternalOperation> Operations { get; }

        public IReadOnlyList<IInternalOperation> CancelOperations { get; }

        public bool IsCancellationAvailable => CancelOperations?.Any() ?? false;

        public OperationGroup(
            IReadOnlyList<IInternalOperation> operations,
            IReadOnlyList<IInternalOperation> cancelOperations = null)
        {
            Operations = operations;
            CancelOperations = cancelOperations;
        }
    }
}