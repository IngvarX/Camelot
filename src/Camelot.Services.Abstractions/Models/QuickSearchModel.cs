using Camelot.Services.Abstractions.Models.Enums;

namespace Camelot.Services.Abstractions.Models;

public class QuickSearchModel
{
    public QuickSearchMode SelectedMode { get; }

    public QuickSearchModel(QuickSearchMode selectedMode)
    {
        SelectedMode = selectedMode;
    }
}