using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camelot.Services.Abstractions;
using Camelot.ViewModels.Implementations.MainWindow.FilePanels;
using Camelot.ViewModels.Services.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.ViewModels.Tests.Services;

public class ClipboardOperationsViewModelTests
{
    private const string Directory = "Dir";
    private const string File = "File";

    private readonly AutoMocker _autoMocker;

    public ClipboardOperationsViewModelTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Fact]
    public void TestCopyToClipboardCommand()
    {
        _autoMocker
            .Setup<INodesSelectionService, IReadOnlyList<string>>(m => m.SelectedNodes)
            .Returns(new[] {File});
        _autoMocker
            .Setup<IClipboardOperationsService>(m => m.CopyFilesAsync(
                It.Is<IReadOnlyList<string>>(l => l.Single() == File)))
            .Verifiable();

        var viewModel = _autoMocker.CreateInstance<ClipboardOperationsViewModel>();

        Assert.True(viewModel.CopyToClipboardCommand.CanExecute(null));

        viewModel.CopyToClipboardCommand.Execute(null);

        _autoMocker
            .Verify<IClipboardOperationsService>(m => m.CopyFilesAsync(
                    It.Is<IReadOnlyList<string>>(l => l.Single() == File)),
                Times.Once);
    }

    [Fact]
    public void TestPasteFromClipboardCommand()
    {
        _autoMocker
            .Setup<IDirectoryService, string>(m => m.SelectedDirectory)
            .Returns(Directory);

        var viewModel = _autoMocker.CreateInstance<ClipboardOperationsViewModel>();

        Assert.True(viewModel.CopyToClipboardCommand.CanExecute(null));

        _autoMocker
            .GetMock<IFilePanelDirectoryObserver>()
            .Raise(m => m.CurrentDirectoryChanged += null, EventArgs.Empty);

        Assert.True(viewModel.PasteFromClipboardCommand.CanExecute(null));

        viewModel.PasteFromClipboardCommand.Execute(null);

        _autoMocker
            .Verify<IClipboardOperationsService>(m => m.PasteFilesAsync(Directory),
                Times.Once);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task TestCanPaste(bool canPaste, bool expected)
    {
        _autoMocker
            .Setup<IClipboardOperationsService, Task<bool>>(m => m.CanPasteAsync())
            .ReturnsAsync(canPaste);

        var viewModel = _autoMocker.CreateInstance<ClipboardOperationsViewModel>();

        var actual = await viewModel.CanPasteAsync();

        Assert.Equal(expected, actual);

        _autoMocker
            .Verify<IClipboardOperationsService, Task<bool>>(m => m.CanPasteAsync(),
                Times.Once);
    }
}