using System.Collections.Generic;

namespace Camelot.Services.Abstractions.Models.State;

public class TabStateModel
{
    public string Directory { get; set; }

    public SortingSettingsStateModel SortingSettings { get; set; }

    public List<string> History { get; set; }

    public int CurrentPositionInHistory { get; set; }

    public TabStateModel()
    {
        History = new List<string>();
    }
}