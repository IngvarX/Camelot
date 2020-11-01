using System.Collections.Generic;

namespace Camelot.DataAccess.Models
{
    public class PanelModel
    {
        public static PanelModel Empty => new PanelModel();

        public List<TabModel> Tabs { get; set; }

        public int SelectedTabIndex { get; set; }

        public PanelModel()
        {
            Tabs = new List<TabModel>();
        }
    }
}