using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Abstractions
{
    public interface ITrashCanService
    {
        Task<bool> MoveToTrashAsync(IReadOnlyCollection<string> nodes);
    }
}