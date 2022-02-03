using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.Views;
using Camelot.Views.Main;

namespace Camelot.Ui.Tests.Common;

public static class ActiveFilePanelProvider
{
    public static FilesPanelViewModel GetActiveFilePanelViewModel(MainWindow window)
    {
        var filesPanel = GetActiveFilePanelView(window);

        return (FilesPanelViewModel) filesPanel.DataContext;
    }

    public static FilesPanelView GetActiveFilePanelView(MainWindow window) =>
        window
            .GetVisualDescendants()
            .OfType<FilesPanelView>()
            .Single(CheckIfActive);

    private static bool CheckIfActive(FilesPanelView filesPanel)
    {
        var dataGrid = GetDataGrid(filesPanel);
        var viewModel = (FilesPanelViewModel) dataGrid.DataContext;

        return viewModel?.IsActive ?? false;
    }

    private static DataGrid GetDataGrid(IVisual filesPanel) =>
        filesPanel
            .GetVisualDescendants()
            .OfType<DataGrid>()
            .Single();
}