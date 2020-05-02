using Camelot.DataAccess.Models;

namespace Camelot.Services.Abstractions
{
    public interface IFilesPanelStateService
    {
        PanelModel GetPanelState();

        void SavePanelState(PanelModel model);
    }
}