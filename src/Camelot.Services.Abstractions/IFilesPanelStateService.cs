using Camelot.Services.Abstractions.Models.State;

namespace Camelot.Services.Abstractions
{
    public interface IFilesPanelStateService
    {
        PanelStateModel GetPanelState();

        void SavePanelState(PanelStateModel model);
    }
}