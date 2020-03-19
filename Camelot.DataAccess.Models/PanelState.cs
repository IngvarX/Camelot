using System.Collections.Generic;

namespace Camelot.DataAccess.Models
{
    public class PanelState
    {
        public List<string> Tabs { get; set; }

        public static PanelState Empty => new PanelState();

        public PanelState()
        {
            Tabs = new List<string>();
        }
    }
}