using Camelot.DataAccess.Models;

namespace Camelot.Services.Interfaces
{
    public interface IFilesPanelStateService
    {
        PanelModel GetPanelState();

        void SavePanelState(PanelModel model);
    }
}