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

namespace Camelot.Ui.Tests.Flows.Directories
{
    public class RemoveDirectoryFlow : IDisposable
    {
        private const string DirectoryName = "RemoveDirectoryTest__Directory";

        private string _directoryFullPath;

        [Fact(DisplayName = "Remove directory")]
        public async Task TestRemoveDirectory()
        {
            var app = AvaloniaApp.GetApp();
            var window = AvaloniaApp.GetMainWindow();

            await FocusFilePanelStep.FocusFilePanelAsync(window);

            var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
            _directoryFullPath = Path.Combine(viewModel.CurrentDirectory, DirectoryName);
            Directory.CreateDirectory(_directoryFullPath);

            ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);

            await Task.Delay(100);

            var filesPanel = ActiveFilePanelProvider.GetActiveFilePanelView(window);
            Assert.NotNull(filesPanel);

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

            OpenRemoveDialogStep.OpenRemoveDialog(window);
            var isRemoveDialogOpened =
                await DialogOpenedCondition.CheckIfDialogIsOpenedAsync<RemoveNodesConfirmationDialog>(app);
            Assert.True(isRemoveDialogOpened);

            Keyboard.PressKey(window, Key.Enter);
            await Task.Delay(100);

            var isRemoveDialogClosed =
                await DialogClosedCondition.CheckIfDialogIsClosedAsync<RemoveNodesConfirmationDialog>(app);
            Assert.True(isRemoveDialogClosed);

            ToggleSearchPanelStep.ToggleSearchPanelVisibility(window);

            Assert.False(Directory.Exists(_directoryFullPath));
        }

        public void Dispose()
        {
            var app = AvaloniaApp.GetApp();
            var dialogs = new Window[]
            {
                DialogProvider.GetDialog<RemoveNodesConfirmationDialog>(app)
            };
            dialogs.ForEach(d => d?.Close());

            if (!string.IsNullOrEmpty(_directoryFullPath) && Directory.Exists(_directoryFullPath))
            {
                Directory.Delete(_directoryFullPath, true);
            }
        }
    }
}