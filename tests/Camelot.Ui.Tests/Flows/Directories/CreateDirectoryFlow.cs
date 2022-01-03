using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Camelot.Extensions;
using Camelot.Ui.Tests.Common;
using Camelot.Ui.Tests.Conditions;
using Camelot.Ui.Tests.Steps;
using Camelot.Views.Dialogs;
using Xunit;

namespace Camelot.Ui.Tests.Flows.Directories;

public class CreateDirectoryFlow : IDisposable
{
    private const string DirectoryName = "CreateDirectoryTest__Directory";

    private string _directoryFullPath;

    [Fact(DisplayName = "Create directory")]
    public async Task CreateDirectoryTest()
    {
        var app = AvaloniaApp.GetApp();
        var window = AvaloniaApp.GetMainWindow();

        await FocusFilePanelStep.FocusFilePanelAsync(window);

        OpenCreateDirectoryDialogStep.OpenCreateDirectoryDialog(window);
        var isDialogOpened = await DialogOpenedCondition.CheckIfDialogIsOpenedAsync<CreateDirectoryDialog>(app);
        Assert.True(isDialogOpened);

        var viewModel = ActiveFilePanelProvider.GetActiveFilePanelViewModel(window);
        _directoryFullPath = Path.Combine(viewModel.CurrentDirectory, DirectoryName);

        CreateDirectoryStep.CreateDirectory(app, window, DirectoryName);

        var isDialogClosed = await DialogClosedCondition.CheckIfDialogIsClosedAsync<CreateDirectoryDialog>(app);
        Assert.True(isDialogClosed);

        Assert.True(Directory.Exists(_directoryFullPath));
    }

    public void Dispose()
    {
        var app = AvaloniaApp.GetApp();
        var dialogs = new Window[]
        {
            DialogProvider.GetDialog<CreateDirectoryDialog>(app)
        };
        dialogs.ForEach(d => d?.Close());

        if (!string.IsNullOrEmpty(_directoryFullPath) && Directory.Exists(_directoryFullPath))
        {
            Directory.Delete(_directoryFullPath, true);
        }
    }
}