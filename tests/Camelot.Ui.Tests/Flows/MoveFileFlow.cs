using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Extensions;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Extensions;
using Camelot.Ui.Tests.Steps;
using Camelot.Views.Dialogs;
using Camelot.Views.Main.Controls;
using Xunit;

namespace Camelot.Ui.Tests.Flows
{
    public class MoveFileFlow : IDisposable
    {
        private const string DirectoryName = "MoveFileTest__Directory";
        private const string FileName = "MoveFileTest__File.txt";
        private const string FileContent = "TestContent1234567890";

        private string _directoryFullPath;
        private string _fileFullPath;

        [Fact(DisplayName = "Move file")]
        public async Task TestCopyFile()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            await FocusFilePanelStep.FocusFilePanelAsync(window);

            CreateNewTabStep.CreateNewTab(window);
            OpenCreateDirectoryDialogStep.OpenCreateDirectoryDialog(window);
            var isDialogOpened = await DialogOpenedCondition.CheckIfDialogIsOpenedAsync<CreateDirectoryDialog>(app);
            Assert.True(isDialogOpened);

            CreateDirectoryStep.CreateDirectory(app, window, DirectoryName);
            var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
            _directoryFullPath = Path.Combine(viewModel.CurrentDirectory, DirectoryName);

            var isDialogClosed = await DialogClosedCondition.CheckIfDialogIsClosedAsync<CreateDirectoryDialog>(app);
            Assert.True(isDialogClosed);

            var filesPanel = ActiveFilePanelProvider.GetActiveFilePanelView(window);
            Assert.NotNull(filesPanel);

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

            searchTextBox.SendText(FileName);

            await Task.Delay(1000);

            ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
            ChangeActiveFilePanelStep.ChangeActiveFilePanel(window);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.Down);

            MoveSelectedNodesStep.MoveSelectedNodes(window);

            ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);
            await Task.Delay(1000);

            var copiedFilePath = Path.Combine(_directoryFullPath, FileName);
            await WaitService.WaitForConditionAsync(() => File.Exists(copiedFilePath));

            var fileContent = await File.ReadAllTextAsync(copiedFilePath);
            Assert.Equal(FileContent, fileContent);

            Assert.False(File.Exists(_fileFullPath));
        }

        public void Dispose()
        {
            var app = AvaloniaApp.GetApp();
            var dialogs = new Window[]
            {
                DialogProvider.GetDialog<CreateDirectoryDialog>(app),
            };
            dialogs.ForEach(d => d?.Close());

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
}