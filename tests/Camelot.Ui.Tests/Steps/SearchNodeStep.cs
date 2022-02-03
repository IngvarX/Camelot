using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Extensions;
using Camelot.Views;
using Camelot.Views.Main.Controls;

namespace Camelot.Ui.Tests.Steps;

public static class SearchNodeStep
{
    public static void SearchNode(MainWindow window, string text)
    {
        var filesPanel = ActiveFilePanelProvider.GetActiveFilePanelView(window);
        var searchPanel = filesPanel
            .GetVisualDescendants()
            .OfType<SearchView>()
            .Single();
        var searchTextBox = searchPanel
            .GetVisualDescendants()
            .OfType<TextBox>()
            .Single();

        searchTextBox.SendText(text);
    }
}