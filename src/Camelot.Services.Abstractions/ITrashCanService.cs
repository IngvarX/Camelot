using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions
{
    public interface ITrashCanService
    {
        Task<bool> MoveToTrashAsync(IReadOnlyList<string> nodes, CancellationToken cancellationToken = default);
    }
}