using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Extensions;
using Camelot.Ui.Tests.Steps;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels.Nodes;
using Camelot.Views.Dialogs;
using Camelot.Views.Main;
using Camelot.Views.Main.Controls;
using Xunit;

namespace Camelot.Ui.Tests.Flows
{
    public class CreateAndRemoveDirectoryFlow : IDisposable
    {
        private const string DirectoryName = "CreateDirectoryTest__Directory";

        private CreateDirectoryDialog _createDirectoryDialog;
        private RemoveNodesConfirmationDialog _removeDialog;
        private string _directoryFullPath;

        [Fact(DisplayName = "Create and remove directory")]
        public async Task CreateAndRemoveDirectoryTest()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            await FocusFilePanelStep.FocusFilePanelAsync(window);

            OpenCreateDirectoryDialogStep.OpenCreateDirectoryDialog(window);
            var isDialogOpened = await DialogOpenedCondition.CheckIfDialogIsOpenedAsync<CreateDirectoryDialog>(app);
            Assert.True(isDialogOpened);

            _createDirectoryDialog = app
                .Windows
                .OfType<CreateDirectoryDialog>()
                .Single();
            var directoryNameTextBox = _createDirectoryDialog
                .GetVisualDescendants()
                .OfType<TextBox>()
                .Single();

            directoryNameTextBox.SendText(DirectoryName);
            Keyboard.PressKey(window, Key.Enter);

            var isDialogClosed = await DialogClosedCondition.CheckIfDialogIsClosedAsync<CreateDirectoryDialog>(app);
            Assert.True(isDialogClosed);

            var filesPanel = window
                .GetVisualDescendants()
                .OfType<FilesPanelView>()
                .SingleOrDefault(CheckIfActive);
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

            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Tab);
            Keyboard.PressKey(window, Key.Down);
            Keyboard.PressKey(window, Key.Down);

            await Task.Delay(100);

            var selectedItemText = GetSelectedItemText(filesPanel);
            Assert.Equal(DirectoryName, selectedItemText);

            Keyboard.PressKey(window, Key.F8);
            await Task.Delay(100);

            _removeDialog = app
                .Windows
                .OfType<RemoveNodesConfirmationDialog>()
                .SingleOrDefault();
            Assert.NotNull(_removeDialog);

            Keyboard.PressKey(window, Key.Enter);
            await Task.Delay(100);

            _removeDialog = app
                .Windows
                .OfType<RemoveNodesConfirmationDialog>()
                .SingleOrDefault();
            Assert.Null(_removeDialog);

            Assert.False(Directory.Exists(_directoryFullPath));
        }

        public void Dispose()
        {
            _createDirectoryDialog?.Close();
            _removeDialog?.Close();

            if (!string.IsNullOrEmpty(_directoryFullPath) && Directory.Exists(_directoryFullPath))
            {
                Directory.Delete(_directoryFullPath);
            }
        }

        private static bool CheckIfActive(FilesPanelView filesPanel)
        {
            var dataGrid = GetDataGrid(filesPanel);
            var viewModel = (FilesPanelViewModel) dataGrid.DataContext;

            return viewModel?.IsActive ?? false;
        }

        private string GetSelectedItemText(IVisual filesPanel)
        {
            var dataGrid = GetDataGrid(filesPanel);
            var directoryViewModel = (DirectoryViewModel) dataGrid.SelectedItem;
            _directoryFullPath = directoryViewModel.FullPath;

            return directoryViewModel.FullName;
        }

        private static DataGrid GetDataGrid(IVisual filesPanel) =>
            filesPanel
                .GetVisualDescendants()
                .OfType<DataGrid>()
                .Single();
    }
}