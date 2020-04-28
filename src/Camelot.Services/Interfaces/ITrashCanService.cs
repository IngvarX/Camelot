using System.Collections.Generic;
using System.Threading.Tasks;

namespace Camelot.Services.Interfaces
{
    public interface ITrashCanService
    {
        Task<bool> MoveToTrashAsync(IReadOnlyCollection<string> files);
    }
}