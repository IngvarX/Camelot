namespace Camelot.ViewModels.Interfaces.MainWindow.FilePanels;

public class QuickSearchCommandModel
{
    public char Symbol { get; init; }

    public bool IsBackwardsDirectionEnabled { get; init; }

    public QuickSearchCommandModel(char symbol, bool isBackwardsDirectionEnabled)
    {
        Symbol = symbol;
        IsBackwardsDirectionEnabled = isBackwardsDirectionEnabled;
    }
}