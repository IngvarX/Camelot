using Camelot.Services.Abstractions.Models.State;

namespace Camelot.Services.Abstractions.Archive
{
    public interface ICreateArchiveStateService
    {
        CreateArchiveStateModel GetState();

        void SaveState(CreateArchiveStateModel model);
    }
}