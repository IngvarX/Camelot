using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Steps;
using Xunit;

namespace Camelot.Ui.Tests.Flows.Files;

public class MoveFileFlow : IDisposable
{
    private const string DirectoryName = "MoveFileTest__Directory";
    private const string FileName = "MoveFileTest__File.txt";
    private const string FileContent = "TestContent1234567890";

    private string _directoryFullPath;
    private string _fileFullPath;

    [Fact(DisplayName = "Move file")]
    public async Task TestMoveFile()
    {
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        CreateNewTabStep.CreateNewTab(window);

        var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
        _directoryFullPath = Path.Combine(viewModel.CurrentDirectory, DirectoryName);
        Directory.CreateDirectory(_directoryFullPath);

        _fileFullPath = Path.Combine(viewModel.CurrentDirectory, FileName);
        await File.WriteAllTextAsync(_fileFullPath, FileContent);

        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        CreateNewTabStep.CreateNewTab(window);
        FocusDirectorySelectorStep.FocusDirectorySelector(window);
        var textSet = SetDirectoryTextStep.SetDirectoryText(window, _directoryFullPath);
        Assert.True(textSet);

        await Task.Delay(1000);

        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);
        await Task.Delay(100);
        SearchNodeStep.SearchNode(window, FileName);

        await Task.Delay(300);
        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        Keyboard.PressKey(window, Key.Down);
        Keyboard.PressKey(window, Key.Down);

        MoveSelectedNodesStep.MoveSelectedNodes(window);

        ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);

        var copiedFullPath = Path.Combine(_directoryFullPath, FileName);
        var fileExists = await WaitService.WaitForConditionAsync(() => File.Exists(copiedFullPath));
        Assert.True(fileExists);

        var fileContent = await File.ReadAllTextAsync(copiedFullPath);
        Assert.Equal(FileContent, fileContent);

        Assert.False(File.Exists(_fileFullPath));
    }

    public void Dispose()
    {
        var window = AvaloniaApp.GetMainWindow();

        for (var i = 0; i < 2; i++)
        {
            ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
            CloseCurrentTabStep.CloseCurrentTab(window);
        }

        if (!string.IsNullOrEmpty(_directoryFullPath) && Directory.Exists(_directoryFullPath))
        {
            Directory.Delete(_directoryFullPath, true);
        }

        if (!string.IsNullOrEmpty(_fileFullPath) && File.Exists(_fileFullPath))
        {
            File.Delete(_fileFullPath);
        }
    }
}