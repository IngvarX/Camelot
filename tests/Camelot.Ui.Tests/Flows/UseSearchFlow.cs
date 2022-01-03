using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Extensions;
using Camelot.Ui.Tests.Steps;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Camelot.Views.Main.Controls;
using Xunit;

namespace Camelot.Ui.Tests.Flows;

public class UseSearchFlow : IDisposable
{
    private const string DirectoryName = "UseSearchFlowTest__Directory";
    private const string FileName = "UseSearchFlowTest__File.txt";

    private string _directoryFullPath;
    private string _fileFullPath;

    [Fact(DisplayName = "Use search")]
    public async Task TestSearch()
    {
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);
        CreateNewTabStep.CreateNewTab(window);

        var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
        _directoryFullPath = Path.Combine(viewModel.CurrentDirectory, DirectoryName);
        Directory.CreateDirectory(_directoryFullPath);

        var filesPanel = ActiveFilePanelProvider.GetActiveFilePanelView(window);
        Assert.NotNull(filesPanel);

        ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);

        await Task.Delay(100);

        var searchPanel = filesPanel
            .GetVisualDescendants()
            .OfType<SearchView>()
            .SingleOrDefault();
        Assert.NotNull(searchPanel);

        var searchTextBox = searchPanel
            .GetVisualDescendants()
            .OfType<TextBox>()
            .SingleOrDefault();
        Assert.NotNull(searchTextBox);

        searchTextBox.SendText(DirectoryName);

        await Task.Delay(1000);

        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        Keyboard.PressKey(window, Key.Down);
        Keyboard.PressKey(window, Key.Down);

        await Task.Delay(100);

        var selectedItemText = GetSelectedItemText(filesPanel);
        Assert.Equal(DirectoryName, selectedItemText);

        _fileFullPath = Path.Combine(viewModel.CurrentDirectory, FileName);
        await File.Create(_fileFullPath).DisposeAsync();

        await Task.Delay(1000);

        var fileIsVisible = CheckIfFilesExist(filesPanel);
        Assert.False(fileIsVisible);
    }

    public void Dispose()
    {
        var window = AvaloniaApp.GetMainWindow();
        ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);
        CloseCurrentTabStep.CloseCurrentTab(window);

        if (!string.IsNullOrEmpty(_directoryFullPath) && Directory.Exists(_directoryFullPath))
        {
            Directory.Delete(_directoryFullPath, true);
        }

        if (!string.IsNullOrEmpty(_fileFullPath) && File.Exists(_fileFullPath))
        {
            File.Delete(_fileFullPath);
        }
    }

    private string GetSelectedItemText(IVisual filesPanel)
    {
        var dataGrid = GetDataGrid(filesPanel);
        var directoryViewModel = (DirectoryViewModel) dataGrid.SelectedItem;
        _directoryFullPath = directoryViewModel.FullPath;

        return directoryViewModel.FullName;
    }

    private static bool CheckIfFilesExist(IVisual filesPanel)
    {
        var dataGrid = GetDataGrid(filesPanel);

        return dataGrid.Items.OfType<FileViewModel>().Any();
    }

    private static DataGrid GetDataGrid(IVisual filesPanel) =>
        filesPanel
            .GetVisualDescendants()
            .OfType<DataGrid>()
            .Single();
}