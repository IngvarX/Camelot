using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Steps;
using Xunit;

namespace Camelot.Ui.Tests.Flows.Directories;

public class CopyDirectoryFlow : IDisposable
{
    private const string SourceDirectoryName = "CopyDirTest__SourceDirectory";
    private const string TargetDirectoryName = "CopyDirTest__TargetDirectory";
    private const string InnerDirectoryName = "CopyDirTest__InnerDirectory";
    private const string EmptyDirectoryName = "CopyDirTest__EmptyDirectory";
    private const string FileName = "CopyDirTest__File.txt";
    private const string FileContent = "TestContent1234";

    private string _sourceDirectoryFullPath;
    private string _targetDirectoryFullPath;

    [Fact(DisplayName = "Copy directory recursively")]
    public async Task TestCopyDirectory()
    {
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        CreateNewTabStep.CreateNewTab(window);

        var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
        _sourceDirectoryFullPath = Path.Combine(viewModel.CurrentDirectory, SourceDirectoryName);
        Directory.CreateDirectory(_sourceDirectoryFullPath);

        var fileFullPath = Path.Combine(_sourceDirectoryFullPath, FileName);
        await File.WriteAllTextAsync(fileFullPath, FileContent);
        var innerDirectoryPath = Path.Combine(_sourceDirectoryFullPath, InnerDirectoryName);
        Directory.CreateDirectory(innerDirectoryPath);
        var innerFileFullPath = Path.Combine(innerDirectoryPath, FileName);
        await File.WriteAllTextAsync(innerFileFullPath, FileContent);
        var emptyDirectoryPath = Path.Combine(innerDirectoryPath, EmptyDirectoryName);
        Directory.CreateDirectory(emptyDirectoryPath);

        _targetDirectoryFullPath = Path.Combine(viewModel.CurrentDirectory, TargetDirectoryName);
        Directory.CreateDirectory(_targetDirectoryFullPath);

        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        CreateNewTabStep.CreateNewTab(window);
        FocusDirectorySelectorStep.FocusDirectorySelector(window);
        var textSet = SetDirectoryTextStep.SetDirectoryText(window, _targetDirectoryFullPath);
        Assert.True(textSet);

        await Task.Delay(1000);

        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        await Task.Delay(100);

        ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);
        await Task.Delay(100);

        SearchNodeStep.SearchNode(window, SourceDirectoryName);
        await Task.Delay(300);

        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
        Keyboard.PressKey(window, Key.Down);
        Keyboard.PressKey(window, Key.Down);

        CopySelectedNodesStep.CopySelectedNodes(window);

        ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);
        await Task.Delay(1000);

        var copiedFullPath = Path.Combine(_targetDirectoryFullPath, SourceDirectoryName, FileName);
        var fileExists = await WaitService.WaitForConditionAsync(() => File.Exists(copiedFullPath));
        Assert.True(fileExists);
        var copiedInnerFullPath = Path.Combine(_targetDirectoryFullPath, SourceDirectoryName, InnerDirectoryName, FileName);
        var innerFileExists = await WaitService.WaitForConditionAsync(() => File.Exists(copiedInnerFullPath));
        Assert.True(innerFileExists);
        var emptyDirPath = Path.Combine(_targetDirectoryFullPath, SourceDirectoryName, InnerDirectoryName, EmptyDirectoryName);
        var emptyDirExists = await WaitService.WaitForConditionAsync(() => Directory.Exists(emptyDirPath));
        Assert.True(emptyDirExists);

        foreach (var filePath in new[] {copiedFullPath, copiedInnerFullPath})
        {
            var fileContent = await File.ReadAllTextAsync(filePath);
            Assert.Equal(FileContent, fileContent);
        }

        Assert.True(File.Exists(fileFullPath));
        Assert.True(File.Exists(innerFileFullPath));
        Assert.True(Directory.Exists(emptyDirectoryPath));
    }

    public void Dispose()
    {
        var window = AvaloniaApp.GetMainWindow();

        for (var i = 0; i < 2; i++)
        {
            ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
            CloseCurrentTabStep.CloseCurrentTab(window);
        }

        if (!string.IsNullOrEmpty(_sourceDirectoryFullPath) && Directory.Exists(_sourceDirectoryFullPath))
        {
            Directory.Delete(_sourceDirectoryFullPath, true);
        }

        if (!string.IsNullOrEmpty(_targetDirectoryFullPath) && Directory.Exists(_targetDirectoryFullPath))
        {
            Directory.Delete(_targetDirectoryFullPath, true);
        }
    }
}