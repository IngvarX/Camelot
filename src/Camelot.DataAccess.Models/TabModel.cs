using System.Collections.Generic;

namespace Camelot.DataAccess.Models;

public class TabModel
{
    public string Directory { get; set; }

    public SortingSettings SortingSettings { get; set; }

    public List<string> History { get; set; }

    public int CurrentPositionInHistory { get; set; }

    public TabModel()
    {
        SortingSettings = new SortingSettings();
        History = new List<string>();
    }
}