using System.Collections.Generic;

namespace Camelot.DataAccess.Models
{
    public class OpenedTabsModel
    {
        public IList<string> LeftPanelTabs { get; set; }
        
        public IList<string> RightPanelTabs { get; set; }
    }
}