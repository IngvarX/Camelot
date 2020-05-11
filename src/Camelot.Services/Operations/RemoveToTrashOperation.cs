using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;

namespace Camelot.Services.Operations
{
    public class RemoveToTrashOperation : OperationBase
    {
        private readonly ITrashCanService _trashCanService;
        private readonly IReadOnlyCollection<string> _nodes;

        public RemoveToTrashOperation(
            ITrashCanService trashCanService,
            IReadOnlyCollection<string> nodes)
        {
            _trashCanService = trashCanService;
            _nodes = nodes;
        }

        public override Task RunAsync(CancellationToken cancellationToken) =>
            _trashCanService.MoveToTrashAsync(_nodes, cancellationToken);
    }
}