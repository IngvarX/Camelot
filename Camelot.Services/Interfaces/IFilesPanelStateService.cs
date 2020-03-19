using Camelot.DataAccess.Models;
using Camelot.Services.Models;

namespace Camelot.Services.Interfaces
{
    public interface IFilesPanelStateService
    {
        PanelState GetPanelState();

        void SavePanelState(PanelState state);
    }
}