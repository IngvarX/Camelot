using System.Collections.Generic;
using System.Threading.Tasks;
using Camelot.Avalonia.Interfaces;
using Camelot.Services.Abstractions.Operations;
using Camelot.Services.Environment.Interfaces;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace Camelot.Services.AllPlatforms.Tests;

public class UnixClipboardOperationsServiceTests
{
    private const string Directory = "Dir";

    private readonly AutoMocker _autoMocker;

    public UnixClipboardOperationsServiceTests()
    {
        _autoMocker = new AutoMocker();
    }

    [Theory]
    [InlineData("file.txt", "file://file.txt")]
    [InlineData("file42.txt", "file://file42.txt")]
    public async Task TestCopy(string sourceFilePath, string clipboardString)
    {
        _autoMocker
            .Setup<IClipboardService>(m => m.SetTextAsync(clipboardString))
            .Verifiable();
        _autoMocker
            .Setup<IEnvironmentService, string>(m => m.NewLine)
            .Returns(System.Environment.NewLine);

        var clipboardOperationsService = _autoMocker.CreateInstance<UnixClipboardOperationsService>();

        await clipboardOperationsService.CopyFilesAsync(new[] {sourceFilePath});

        _autoMocker
            .Verify<IClipboardService>(m => m.SetTextAsync(clipboardString), Times.Once);
    }

    [Theory]
    [InlineData("file.txt", "file://file.txt")]
    [InlineData("file42.txt", "file://file42.txt")]
    public async Task TestPaste(string sourceFilePath, string clipboardString)
    {

        _autoMocker
            .Setup<IClipboardService, Task<string>>(m => m.GetTextAsync())
            .ReturnsAsync(clipboardString);
        _autoMocker
            .Setup<IOperationsService>(m => m.CopyAsync(new[] {sourceFilePath}, Directory))
            .Verifiable();

        var clipboardOperationsService = _autoMocker.CreateInstance<UnixClipboardOperationsService>();

        await clipboardOperationsService.PasteFilesAsync(Directory);

        _autoMocker
            .Verify<IOperationsService>(m => m.CopyAsync(new[] {sourceFilePath}, Directory), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(" \t   \t\n")]
    [InlineData("test")]
    public async Task TestPasteInvalidData(string text)
    {
        _autoMocker
            .Setup<IClipboardService, Task<string>>(m => m.GetTextAsync())
            .ReturnsAsync(text);

        var clipboardOperationsService = _autoMocker.CreateInstance<UnixClipboardOperationsService>();

        await clipboardOperationsService.PasteFilesAsync(Directory);

        _autoMocker
            .Verify<IOperationsService>(m => m.CopyAsync(It.IsAny<IReadOnlyList<string>>(), Directory),
                Times.Never);
    }

    [Theory]
    [InlineData("file://file.txt", true)]
    [InlineData("file://file42.txt", true)]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData(" \t   \t\n", false)]
    [InlineData("test", false)]
    public async Task TestCanPaste(string clipboardString, bool expected)
    {

        _autoMocker
            .Setup<IClipboardService, Task<string>>(m => m.GetTextAsync())
            .ReturnsAsync(clipboardString);

        var clipboardOperationsService = _autoMocker.CreateInstance<UnixClipboardOperationsService>();

        var canPaste = await clipboardOperationsService.CanPasteAsync();

        Assert.Equal(expected, canPaste);
    }
}