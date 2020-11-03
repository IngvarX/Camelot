using System.Collections.Generic;

namespace Camelot.Services.Abstractions.Models.State
{
    public class PanelStateModel
    {
        public IReadOnlyList<TabStateModel> Tabs { get; set; }

        public int SelectedTabIndex { get; set; }
    }
}