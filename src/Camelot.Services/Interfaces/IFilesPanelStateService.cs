using Camelot.DataAccess.Models;

namespace Camelot.Services.Interfaces
{
    public interface IFilesPanelStateService
    {
        PanelState GetPanelState();

        void SavePanelState(PanelState state);
    }
}