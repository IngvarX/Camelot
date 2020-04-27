using System.Collections.Generic;

namespace Camelot.Services.Interfaces
{
    public interface ITrashCanLocator
    {
        IReadOnlyCollection<string> GetTrashCanDirectories(string volume);
    }
}