using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Interfaces.MainWindow.FilePanels;
using Camelot.Views.Main;
using Xunit;

namespace Camelot.Ui.Tests.Flows
{
    public class CheckHistoryFlow : IDisposable
    {
        private string _directoryFullPath;

        [Fact(DisplayName = "Go to parent directory and back using history")]
        public async Task GoToParentDirectoryAndBackTest()
        {
            var window = AvaloniaApp.GetMainWindow();

            await Task.Delay(100);

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.Up);
            Keyboard.PressKey(window, Key.Up);

            var filesPanel = window
                .GetVisualDescendants()
                .OfType<FilesPanelView>()
                .SingleOrDefault(CheckIfActive);
            Assert.NotNull(filesPanel);

            var viewModel = (FilesPanelViewModel) filesPanel.DataContext;
            Assert.NotNull(viewModel);

            _directoryFullPath = viewModel.CurrentDirectory;

            Keyboard.PressKey(window, Key.Enter);

            var parentDirectoryWasOpened = await WaitService.WaitForConditionAsync(() =>
                CheckIfParentDirectoryWasOpened(viewModel, _directoryFullPath));
            Assert.True(parentDirectoryWasOpened);

            Keyboard.PressKey(window, Key.Left);
            var childDirectoryWasOpened = await WaitService.WaitForConditionAsync(() =>
                CheckIfDirectoryWasOpened(viewModel, _directoryFullPath));
            Assert.True(childDirectoryWasOpened);

            for (var i = 0; i < 10; i++)
            {
                Keyboard.PressKey(window, Key.Right);
                var parentDirectoryWasReopened = await WaitService.WaitForConditionAsync(() =>
                    CheckIfParentDirectoryWasOpened(viewModel, _directoryFullPath));
                Assert.True(parentDirectoryWasReopened);
            }
        }

        public void Dispose()
        {
            if (_directoryFullPath is null)
            {
                return;
            }

            var window = AvaloniaApp.GetMainWindow();
            var filesPanel = window
                .GetVisualDescendants()
                .OfType<FilesPanelView>()
                .Single(CheckIfActive);
            var viewModel = (FilesPanelViewModel) filesPanel.DataContext;
            viewModel.CurrentDirectory = _directoryFullPath;
        }

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

        private static bool CheckIfParentDirectoryWasOpened(IFilesPanelViewModel viewModel, string directoryPath) =>
            CheckIfDirectoryWasOpened(viewModel, Directory.GetParent(directoryPath).FullName);

        private static bool CheckIfDirectoryWasOpened(IFilesPanelViewModel viewModel, string directoryPath) =>
            viewModel.CurrentDirectory == directoryPath;
    }
}