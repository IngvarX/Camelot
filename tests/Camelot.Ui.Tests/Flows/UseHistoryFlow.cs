using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Steps;
using Xunit;

namespace Camelot.Ui.Tests.Flows
{
    public class UseHistoryFlow : IDisposable
    {
        private string _directoryFullPath;

        [Fact(DisplayName = "Go to parent directory and back using history")]
        public async Task GoToParentDirectoryAndBackTest()
        {
            var window = AvaloniaApp.GetMainWindow();

            await FocusFilePanelStep.FocusFilePanelAsync(window);
            var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);

            _directoryFullPath = viewModel.CurrentDirectory;

            GoToParentDirectoryStep.GoToParentDirectoryViaFilePanel(window);
            var isParentDirectoryOpened = await DirectoryOpenedCondition.CheckIfParentDirectoryIsOpenedAsync(window, _directoryFullPath);
            Assert.True(isParentDirectoryOpened);

            Keyboard.PressKey(window, Key.Left);
            var isChildDirectoryOpened = await DirectoryOpenedCondition.CheckIfDirectoryIsOpenedAsync(window, _directoryFullPath);
            Assert.True(isChildDirectoryOpened);

            for (var i = 0; i < 10; i++)
            {
                Keyboard.PressKey(window, Key.Right);
                var parentDirectoryWasReopened = await DirectoryOpenedCondition.CheckIfParentDirectoryIsOpenedAsync(window,
                    _directoryFullPath);
                Assert.True(parentDirectoryWasReopened);
            }

            Keyboard.PressKey(window, Key.Left);
            var isChildDirectoryReopened = await DirectoryOpenedCondition.CheckIfDirectoryIsOpenedAsync(window, _directoryFullPath);
            Assert.True(isChildDirectoryReopened);
        }

        public void Dispose()
        {
            if (_directoryFullPath is null)
            {
                return;
            }

            var window = AvaloniaApp.GetMainWindow();
            var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
            viewModel.CurrentDirectory = _directoryFullPath;
        }
    }
}